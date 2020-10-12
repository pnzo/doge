using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ASTRALib;

namespace Doge.Model.RastrPart
{
    public class RastrOperations
    {
        private Rastr _rastr;

        public RastrOperations()
        {
            _rastr = new Rastr();
        }

        ~RastrOperations()
        {
            _rastr = null;
        }

        public static string FindTemplatePathWithExtension(string extension)
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                  @"\RastrWIN3\SHABLON\")) return null;
            var files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RastrWIN3\SHABLON\");
            return files.FirstOrDefault(filename => Path.GetExtension(filename) == extension);
        }

        public void Load(params string[] files)
        {
            foreach (var file in files)
            {
                _rastr.Load(RG_KOD.RG_REPL, file, FindTemplatePathWithExtension(Path.GetExtension(file)));
            }

        }

        public void Save(string file)
        {
            _rastr.Save(file, FindTemplatePathWithExtension(Path.GetExtension(file)));
        }

        public void SetValueWithCalc(string tableName, string columnName, string selection, string value)
        {
            table table = _rastr.Tables.Item(tableName);
            col column = table.Cols.Item(columnName);
            table.SetSel(selection);
            column.Calc(value);
        }

        public void SetValue(string tableName, string columnName, string selection, string value)
        {
            table table = _rastr.Tables.Item(tableName);
            col column = table.Cols.Item(columnName);
            table.SetSel(selection);
            var j = table.FindNextSel[-1];
            while (j != -1)
            {
                column.Z[j] = value;
                j = table.FindNextSel[j];
            }
        }

        public void SetValue(string tableName, string columnName, int index, dynamic value)
        {
            table table = _rastr.Tables.Item(tableName);
            col column = table.Cols.Item(columnName);
            column.Z[index] = value;
        }


        public int AddRow(string table)
        {
            table t = _rastr.Tables.Item(table);
            t.AddRow();
            return t.Count - 1;
        }

        public dynamic GetValue(string tableName, string columnName, string selection)
        {
            table table = _rastr.Tables.Item(tableName);
            col column = table.Cols.Item(columnName);
            table.SetSel(selection);
            var j = table.FindNextSel[-1];
            if (j != -1)
            {
                return column.Z[j];
            }
            else
            {
                return null;
            }
        }


        public bool RunRGM(string parameter = @"")
        {
            var result = _rastr.rgm(parameter) == RastrRetCode.AST_OK;
            return result;
        }

        public void ApplyRastrElement(RastrElement rastrElement)
        {
            var state = rastrElement.State ? @"0" : @"1";
            if (rastrElement.IsBranch)
                SetValueWithCalc(@"vetv", @"sta", rastrElement.Selection, state);
            else
                SetValueWithCalc(@"node", @"sta", rastrElement.Selection, state);
        }

        public bool InitializeLoading()
        {
            var code = _rastr.step_ut(@"i");
            return code == RastrRetCode.AST_OK;
        }

        public bool PerformStep()
        {
            var code = _rastr.step_ut(@"z");
            return code == RastrRetCode.AST_OK;
        }
    }
}
