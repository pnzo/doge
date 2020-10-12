using Doge.Model.Calculation;
using Doge.Model.RastrPart;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doge.Model.Result
{
    public class Result
    {
        public List<ResultRepair> ResultRepairs { get; set; }
        public Result(Calculator calculator, List<BranchElementWithCurrentLimits> branchElementsWithCurrentLimits)
        {
            ResultRepairs = new List<ResultRepair>();
            var temperatures = new List<double>();
            var firstBranchElementWithCurrentLimits = branchElementsWithCurrentLimits[0];
            foreach (var currentLimit in firstBranchElementWithCurrentLimits.CurrentLimits)
                temperatures.Add(currentLimit.Temperature);
            foreach (var repair in calculator.Repairs)
            {
                var resultRepair = new ResultRepair();
                resultRepair.Name = repair.Name;
                resultRepair.Plimit = Math.Floor(repair.Accidents[0].PowerValues.Max());
                resultRepair.P8 = Math.Floor(resultRepair.Plimit * 0.92);
                resultRepair.P20 = Math.Floor(resultRepair.Plimit * 0.8 - repair.DeltaP);
                foreach (var temperature in temperatures)
                {
                    var resultTemperature = new ResultTemperature();
                    resultTemperature.TemperatureValue = temperature;
                    var longTermOverload = repair.Accidents[0].GetLongTermOverload(temperature, branchElementsWithCurrentLimits);
                    if (longTermOverload != null)
                    {
                        resultTemperature.Pddtn = Math.Floor((repair.GetPowerValueByStep(longTermOverload.Step) ?? 0) - repair.DeltaP);
                        resultTemperature.CriteriaOfPddtn = longTermOverload.branchElementWithCurrentLimit.Name;
                        resultTemperature.DDTN = Math.Ceiling(longTermOverload.CurrentLimitValue);
                    }

                    var shortTermOverload = repair.Accidents[0].GetShortTermOverload(temperature, branchElementsWithCurrentLimits);
                    if (shortTermOverload != null)
                    {
                        resultTemperature.Padtn = Math.Floor((repair.GetPowerValueByStep(shortTermOverload.Step) ?? 0) - repair.DeltaP);
                        resultTemperature.CriteriaOfPadtn = shortTermOverload.branchElementWithCurrentLimit.Name;
                        resultTemperature.ADTN = Math.Ceiling(shortTermOverload.CurrentLimitValue);
                    }

                    foreach (var accident in repair.Accidents)
                    {
                        if ((accident.PowerValues[0] == 0) || (accident.Name == @""))
                            continue;

                        var resultAccident = new ResultAccident();
                        resultAccident.Name = accident.Name;
                        resultAccident.Plimit = Math.Floor(accident.PowerValues.Max());
                        resultAccident.P8 = Math.Floor(resultAccident.Plimit * 0.92);
                        var shortTermOverloadInAccident = accident.GetShortTermOverload(temperature, branchElementsWithCurrentLimits);
                        if (shortTermOverloadInAccident != null)
                        {
                            resultAccident.Padtn = Math.Floor((repair.GetPowerValueByStep(shortTermOverloadInAccident.Step) ?? 0) - repair.DeltaP);
                            resultAccident.CriteriaOfPadtn = shortTermOverloadInAccident.branchElementWithCurrentLimit.Name;
                            resultAccident.ADTN = Math.Ceiling(shortTermOverloadInAccident.CurrentLimitValue);
                        }
                        var p8BeforeAccidentStep = accident.GetStepForPowerValue(resultAccident.P8);
                        resultAccident.P8BeforeAccident = Math.Floor((repair.GetPowerValueByStep(p8BeforeAccidentStep) ?? 0) - repair.DeltaP);
                        resultTemperature.ResultAccidents.Add(resultAccident);
                    }

                    resultRepair.ResultTemperatures.Add(resultTemperature);


                }
                ResultRepairs.Add(resultRepair);
            }
        }

        public void SaveToExcel(string filename)
        {
            var templateFile = Environment.CurrentDirectory + @"\template.xlsx";
            using (var package = new ExcelPackage(new FileInfo(templateFile)))
            {
                var sheet = package.Workbook.Worksheets[0];
                var counter = 4;
                var repairCounter = 1;
                foreach (var resultRepair in ResultRepairs)
                {

                    var temperatureCounter = 1;
                    sheet.Cells[counter, 1].Value = repairCounter;
                    sheet.Cells[counter, 2].Value = resultRepair.Name;

                    sheet.Cells[counter, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheet.Cells[counter, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[counter, 1].Style.Fill.BackgroundColor.SetColor(Color.Cyan);
                    sheet.Cells[counter, 1].Style.Font.Bold = true;
                    sheet.Cells[counter, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet.Cells[counter, 2].Style.Fill.BackgroundColor.SetColor(Color.Cyan);
                    sheet.Cells[counter, 2].Style.Font.Bold = true;
                    sheet.Cells[counter, 2, counter, 20].Merge = true;
                    counter++;
                    var firstRepairRow = counter;
                    foreach (var resultTemperature in resultRepair.ResultTemperatures)
                    {
                        sheet.Cells[counter, 1].Value = $@"{repairCounter}.{temperatureCounter}";
                        sheet.Cells[counter, 2].Value = resultTemperature.TemperatureValue;
                        sheet.Cells[counter, 3].Value = resultTemperature.Pddtn;
                        sheet.Cells[counter, 4].Value = resultTemperature.CriteriaOfPddtn;
                        sheet.Cells[counter, 5].Value = resultTemperature.DDTN;
                        sheet.Cells[counter, 6].Value = resultTemperature.Padtn;
                        sheet.Cells[counter, 7].Value = resultTemperature.CriteriaOfPadtn;
                        sheet.Cells[counter, 8].Value = resultTemperature.ADTN;
                        sheet.Cells[counter, 9].Value = resultRepair.Plimit;
                        sheet.Cells[counter, 10].Value = resultRepair.P8;
                        sheet.Cells[counter, 11].Value = resultRepair.P20;

                        
                        sheet.Cells[counter, 19].Value = Math.Min(resultRepair.P20,resultTemperature.MDP);
                        sheet.Cells[counter, 20].Value = Math.Min(resultRepair.P20, resultTemperature.Pddtn ?? double.MaxValue);

                        for (int i = 1; i <= 11; i++)
                            sheet.Cells[counter, i, counter + resultTemperature.ResultAccidents.Count - 1, i].Merge = true;
                        for (int i = 19; i <= 20; i++)
                            sheet.Cells[counter, i, counter + resultTemperature.ResultAccidents.Count - 1, i].Merge = true;


                        foreach (var resultAccident in resultTemperature.ResultAccidents)
                        {
                            sheet.Cells[counter, 12].Value = resultAccident.Name;
                            sheet.Cells[counter, 13].Value = resultAccident.Padtn;
                            sheet.Cells[counter, 14].Value = resultAccident.CriteriaOfPadtn;
                            sheet.Cells[counter, 15].Value = resultAccident.ADTN;
                            sheet.Cells[counter, 16].Value = resultAccident.Plimit;
                            sheet.Cells[counter, 17].Value = resultAccident.P8;
                            sheet.Cells[counter, 18].Value = resultAccident.P8BeforeAccident;
                            counter++;
                        }
                        temperatureCounter++;
                    }
                    for (int i = firstRepairRow; i < counter; i++)
                    {
                        sheet.Row(i).OutlineLevel = 1;
                        sheet.Row(i).Collapsed = true;
                    }
                    sheet.Cells[firstRepairRow, 1, counter - 1, 11].Style.WrapText = true;
                    sheet.Cells[firstRepairRow - 1, 1, counter - 1, 20].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[firstRepairRow - 1, 1, counter - 1, 20].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[firstRepairRow - 1, 1, counter - 1, 20].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[firstRepairRow - 1, 1, counter - 1, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    sheet.Cells[firstRepairRow, 1, counter - 1, 20].Style.Font.Name = @"Times New Roman";
                    sheet.Cells[firstRepairRow, 1, counter - 1, 20].Style.Font.Size = 8;
                    sheet.Cells[firstRepairRow, 1, counter - 1, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[firstRepairRow, 1, counter - 1, 20].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sheet.Cells[firstRepairRow, 4, counter - 1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheet.Cells[firstRepairRow, 7, counter - 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheet.Cells[firstRepairRow, 12, counter - 1, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheet.Cells[firstRepairRow, 14, counter - 1, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    sheet.OutLineApplyStyle = true;
                    repairCounter++;
                }



                package.SaveAs(new FileInfo(filename));
            }
        }
    }
}
