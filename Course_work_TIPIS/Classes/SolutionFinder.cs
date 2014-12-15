using System;
using System.Diagnostics;

namespace Course_work_TIPIS
{
    //This class looks for a solution. 
    //Also it considers a case, when we get only a (n-1) solutions via simplex-algorithm
    public static class SolutionFinder
    {
        #region Get a solutions from solution-matrix
        //Take a values from the Base column of solution-table 
        private static object[,] GetBaseCol(object[,] _simplexTable)
        {
            object[,] solutionValues = new object[2, _simplexTable.GetLength(0) - 1];

            //Produce a getting of names and values (name - the first row, values - the second row)
            //i = 1, because we don't  take a row with titles
            for (int i = 1, counter = 0; i < _simplexTable.GetLength(0); i++, counter++)
            {
                solutionValues[0, counter] = _simplexTable[i, 0];
                solutionValues[1, counter] = _simplexTable[i, 1];
            }

            return solutionValues;
        }

        //A checking on finish of our calculations via simplex-algorithm
        private static string SimplexSolved(object[,] _solutionValues, object[,] _targetFunction)
        {
            object[,] flagArray = new object[2, _targetFunction.GetLength(1)];
            //fill flagArray
            for (int j = 0; j < flagArray.GetLength(1); j++)
            {
                flagArray[0, j] = _targetFunction[0, j]; //fill the names of variables
                flagArray[1, j] = 0; //fill the values of flags
            }
            
            //Setup of flags
            for (int i = 0; i < flagArray.GetLength(1); i++)
            {
                for (int j = 0; j < _solutionValues.GetLength(1); j++)
                {
                    if (flagArray[0, i].ToString() == _solutionValues[0, j].ToString())
                        flagArray[1, i] = 1;
                }
            }

            //Get an unknown variable
            string unknownVariable = null;

            for (int j = 0; j < flagArray.GetLength(1); j++)
                if (Convert.ToDouble(flagArray[1, j]) == 0)
                    unknownVariable = flagArray[0, j].ToString();

            return unknownVariable;
        }
        #endregion

        #region Form search-matrix
        //Setting of values
        private static void SetValueToField(object[,] _findingMatrix, int _j, object[,] _solutionValues)
        {
            for (int k = 0; k < _solutionValues.GetLength(1); k++)
            {
                if (_findingMatrix[0, _j].ToString() == _solutionValues[0, k].ToString())
                {
                    _findingMatrix[1, _j] = _solutionValues[1, k];
                }
            }
        }

        //Form a matrix for calculation of unknown value of unknown variable
        private static object[,] CreateFindingMatrix(object[,] _targetFunction, object[,] _solutionValues)
        {
            /*+++++++Matrix view+++++++*/
            //The variables name
            //Values
            //Coefficients
            object[,] findingMatrix = new object[3, _targetFunction.GetLength(1) + 1]; //+1, because we consider a values of free members

            for (int j = 0; j < findingMatrix.GetLength(1); j++) 
            {
                //Fill a free member value 
                if (j == findingMatrix.GetLength(1) - 1) 
                {
                    findingMatrix[0, j] = "z";
                    findingMatrix[1, j] = (-1) * Convert.ToDouble(_solutionValues[1, _solutionValues.GetLength(1) - 1]);
                    findingMatrix[2, j] = 0;
                }
                else
                {
                    findingMatrix[0, j] = _targetFunction[0, j]; //fill the name
                    SetValueToField(findingMatrix, j, _solutionValues); //fill known variables
                    findingMatrix[2, j] = _targetFunction[1, j]; //fill coeficients
                }
            }
            
            //set sign "?" on the space of unknown variable
            for (int j = 0; j < findingMatrix.GetLength(1); j++)
            {
                if (findingMatrix[1, j] == null)
                    findingMatrix[1, j] = "?";
            }

            return findingMatrix;
        }
        #endregion

        #region Search of a value of unknown variable
        
        //Look for the number of a column with unknown variable in searching-matrix
        private static int GetUnknownVarCol(object[,] _findingMatrix, string _unknownVariable)
        {
            for (int j = 0; j < _findingMatrix.GetLength(1); j++)
                if (_findingMatrix[0, j].ToString() == _unknownVariable)
                    return j;    
                
            return -1;
        }

        //Expression and search of unknown variable
        private static void SetUnknownValue(object[,] _findingMatrix, string _unknownVariable)
        {
            double value = Convert.ToDouble(_findingMatrix[1, _findingMatrix.GetLength(1) - 1]); //значение z
            int k = GetUnknownVarCol(_findingMatrix, _unknownVariable);

            for (int j = 0; j < _findingMatrix.GetLength(1); j++)
            {
                //If column includes a known variable
                if (k != j)
                {
                    value -= (Convert.ToDouble(_findingMatrix[1, j]) * Convert.ToDouble(_findingMatrix[2, j]));
                }
                //in opposite way
                else
                    continue;
            }
            
            //Return an unknown value of a variable
            value = value/Convert.ToDouble(_findingMatrix[2,k]);

            //Set a value on the place of sign "?" in matrix
            for (int j = 0; j < _findingMatrix.GetLength(1); j++)
            {
                if (_findingMatrix[1, j].ToString() == "?")
                    _findingMatrix[1, j] = value;
            }
        }

        //Calculation of unknown value and recor it to matrix
        private static object[,] CalculateUnknownVariableInMatrix(object[,] _simplexTable, object[,] _targetFunction)
        {
            object[,] solutionValues = GetBaseCol(_simplexTable);
            string unknownVariable = SimplexSolved(solutionValues, _targetFunction);
            object[,] findingMatrix = CreateFindingMatrix(_targetFunction, solutionValues);

            if(SimplexSolved(solutionValues, _targetFunction) != null)
                SetUnknownValue(findingMatrix, unknownVariable);
            
            Service.TracingObjectMatrix(solutionValues);
            
            return findingMatrix;
        }
        #endregion

        //Getting of a solution
        public static object[,] GetSolution(object[,] _simplexTable, object[,] _targetFunction, ref double minVertex) 
        {
            object[,] solutionMatrix = new object[2, _targetFunction.GetLength(1)];
            object[,] findingMatrix = CalculateUnknownVariableInMatrix(_simplexTable, _targetFunction);

            //Fill the solution-matrix
            for (int j = 0; j < solutionMatrix.GetLength(1); j++)
            {
                solutionMatrix[0, j] = findingMatrix[0, j]; //the name of variables
                solutionMatrix[1, j] = findingMatrix[1, j]; //the values of variables

                if (solutionMatrix[1, j].ToString().Contains("E-"))
                    solutionMatrix[1, j] = 0;
            }

            //Service.TracingObjectMatrix(findingMatrix);

            minVertex = Convert.ToDouble(findingMatrix[1, findingMatrix.GetLength(1) - 1]);

            return solutionMatrix;
        }
    }
}
