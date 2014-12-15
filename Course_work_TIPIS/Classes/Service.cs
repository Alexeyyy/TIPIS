using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Diagnostics;


namespace Course_work_TIPIS
{
    //A special class which concludes a methods for service: trace, records and etc.
    class Service
    {
        public static List<Table> solutionTables = new List<Table>();
        public static string solutionString = "";

        #region Copying data...
        //Create a safe copy of list
        public static void MakeSafeCopy(List<object[,]> _copiedList, List<object[,]> _recordedList)
        {
            for (int i = 0; i < _copiedList.Count; i++)
            {
                object[,] tmpItem = _copiedList[i];
                _recordedList.Add(tmpItem);
            }
        }
        #endregion

        #region Traces...

        //Trace a matrix which  type is object[,]
        public static void TracingObjectMatrix(object[,] _matrix)
        {
            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                Trace.WriteLine("");
                for (int j = 0; j < _matrix.GetLength(1); j++)
                {
                    if (_matrix[i, j].ToString().Contains("E-"))
                        Trace.Write(String.Format("{0,20}", "0"));
                    else
                        Trace.Write(String.Format("{0,20}", _matrix[i, j]));
                }
            }
            Trace.WriteLine("");
        }

        //Trace a list of conditions which type is List<object[,]>
        public static void TracingConditionsList(List<object[,]> _list)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                Trace.WriteLine("Условие " + i + "\n");
                for (int j = 0; j < _list[i].GetLength(0); j++)
                {
                    Trace.WriteLine("");
                    for (int k = 0; k < _list[i].GetLength(1); k++)
                    {
                        Trace.Write(String.Format("{0,16}", _list[i][j, k] + " "));
                    }
                }
                Trace.WriteLine("");
            }
        }



        #endregion

        #region Record an order of decision

        //Record a step-matrix
        public static void RecordMatrix(object[,] _matrix)
        {
            solutionTables.Add(new Table());
            solutionTables[solutionTables.Count - 1].RowGroups.Add(new TableRowGroup());
            for (int i = 0; i < _matrix.GetLength(0); i++)
            {
                //add a new row in table
                solutionTables[solutionTables.Count - 1].RowGroups[0].Rows.Add(new TableRow());;
                
                for (int j = 0; j < _matrix.GetLength(1); j++)
                {
                    if (_matrix[i, j].ToString().Contains("E-"))
                        solutionTables[solutionTables.Count - 1].RowGroups[0].Rows[i].Cells.Add(new TableCell(new Paragraph(new Run("0"))));
                    else
                        solutionTables[solutionTables.Count - 1].RowGroups[0].Rows[i].Cells.Add(new TableCell(new Paragraph(new Run(_matrix[i, j].ToString()))));
                }
            }
        }

        //Record solution and represent it in string form
        public static void RecordSolution(object[,] _solutionMatrix, double _minValue)
        {
            solutionString = "Результат поиска минимума после применения симплекс-алгоритма:\n\n";

            //Write received values in string
            for (int j = 0; j < _solutionMatrix.GetLength(1); j++)
            {
                if (j != _solutionMatrix.GetLength(1) - 1)
                    solutionString += (_solutionMatrix[0, j] + " = " + _solutionMatrix[1, j] + ", ");
                else
                    solutionString += (_solutionMatrix[0, j] + " = " + _solutionMatrix[1, j]);
            }

            //Write a minimum in point too
            solutionString += ("\nЗначение в точке min равно " + _minValue + " (Fmin = " + _minValue + ").");
        }
        
        #endregion
    }
}
