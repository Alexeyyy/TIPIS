using System;
using System.Linq;

namespace Course_work_TIPIS
{
    public static class SimplexAlgorithm
    {
        //The main method which realises a work of simplex-algorithm
        public static void Simplex(ref object[,] _matrix)
        {
            BuildBase(_matrix);
            WorksWithArtificialFunction(ref _matrix);
            WorksWithTargetFunction(ref _matrix);
        }

        #region STEP 1 "Build a base on the foundation of fective and artificial variables"
        
        //Build a base for solution
        private static void BuildBase(object[,] _matrix)
        {
            for (int i = 1; i < _matrix.GetLength(0); i++)
            {
                if ((string)_matrix[i, 0] == "x")
                {
                    //j = 2, because the variables appear from 2nd col in the table
                    for (int j = 2; j < _matrix.GetLength(1); j++)
                    {
                        if (Convert.ToDouble(_matrix[i, j]) == 1 && CheckNull(_matrix, i, j))
                        {
                            _matrix[i, 0] = _matrix[0, j];
                            break;
                        }
                    }
                }
            }
        }

        //Check a column on 0s
        private static bool CheckNull(object[,] _matrix, int _numRow, int _numCol)
        {
            if (((string)_matrix[0, _numCol]).Contains('x')) //variables x cannot make a base
                return false;
            for (int i = _matrix.GetLength(0) - 1; i > 0; i--)
            {
                if (Convert.ToDouble(_matrix[i, _numCol]) == 0 || Convert.ToDouble(_matrix[_numRow, _numCol]) == 1)
                    continue;
                else
                    return false;
            }
            return true;
        }
        #endregion

        #region STEP 3 "Checkings on exit of working simplex-algorithm with artificial function"
        //Get a count of artificial variables
        private static int FindArtificialVariablesCount(object[,] _matrix)
        {
            int artCounter = 0;

            for (int i = 0; i < _matrix.GetLength(1); i++)
                if ((_matrix[0, i] as string).Contains("art"))
                    artCounter++;
            
            return artCounter;
        }

        //Checking artificial variables on equality of 1 and another variables of 0 
        private static bool CheckArtificialVariables(object[,] _matrix)
        {
            //j = 2, because don't consider "-w", also in this case, don't consider a count of artificial variables
            for (int j = 2; j < _matrix.GetLength(1) - FindArtificialVariablesCount(_matrix); j++)
            {
                if (Convert.ToDouble(_matrix[_matrix.GetLength(0) - 1, j]) == 0)
                    continue;
                else
                    return false;
            }
            return true;
        }
        #endregion

        #region Step 3 "An implementation of the main work with matrix"

        #region Work with an artificial function

        //If we produce a transfer from artificial function to target one, so resave our matrix
        private static object[,] RepresentMatrix(ref object[,] _matrixBefore)
        {
            object[,] matrixAfter = new object[_matrixBefore.GetLength(0) - 1, _matrixBefore.GetLength(1) - FindArtificialVariablesCount(_matrixBefore)];

            for (int i = 0; i < matrixAfter.GetLength(0); i++)
                for (int j = 0; j < matrixAfter.GetLength(1); j++)
                    matrixAfter[i, j] = _matrixBefore[i, j];

            return matrixAfter;
        }

        //Provide a liberation from artificial function
        private static void WorksWithArtificialFunction(ref object[,] _matrix)
        {
            while(!CheckArtificialVariables(_matrix)) {
                //Service.TracingObjectMatrix(_matrix);
                Service.RecordMatrix(_matrix);

                //Look for a minimum in the row of artificial function (the first minimum)
                int j_min = FindMinimumElColumn(_matrix);
                //Find a lead string
                int i_min = FindNumberLeadRow(_matrix, j_min, 2);
                //Trace.WriteLine("Ведущий элемент " + i_min + " " + j_min);
                //Service.RecordString("Ведущий элемент " + i_min + " " + j_min);
                
                //Change a base (x_i1 = x_0j)
                ChangeBase(_matrix, i_min, j_min);
                //If (x_ij != 1) then devide all row elements on x_ij 
                RowDevision(_matrix, i_min, j_min);
                //Set 0-s in a col (by calculation way)
                SetNullsToColumn(_matrix, i_min, j_min);
            }
            //Service.TracingObjectMatrix(_matrix);
            Service.RecordMatrix(_matrix);

            //Delete a row with artificial function and cols with artificial variables
            _matrix = RepresentMatrix(ref _matrix);
            //Trace.WriteLine("Переход");
            //Service.TracingObjectMatrix(_matrix);
            Service.RecordMatrix(_matrix);
        }
        
        #endregion
        
        #region Work with a target fucntion
        
        //A condition of exit from simplex-algorithm 
        private static bool CheckingTargetFunctionEls(object[,] _matrix)
        {
            //j = 2, because j = 0 - is a base, j = 1 - values
            for (int j = 2; j < _matrix.GetLength(1); j++)
            {
                if (Convert.ToDouble(_matrix[_matrix.GetLength(0) - 1, j]) < 0)
                    return false;
                else
                    continue;
            }
            return true;
        }

        //Find a solution
        private static void WorksWithTargetFunction(ref object[,] _matrix)
        {
            while (!CheckingTargetFunctionEls(_matrix)) 
            {
                Service.TracingObjectMatrix(_matrix);
                Service.RecordMatrix(_matrix);
                //Find a minimum in a row of target fucntion (the first minimum)
                int j_min = FindMinimumElColumn(_matrix);
                //Find a lead row
                int i_min = FindNumberLeadRow(_matrix, j_min, 1);
                //Trace.WriteLine("Ведущий элемент " + i_min + " " + j_min);
                               
                //Change a base (x_i1 = x_0j)
                ChangeBase(_matrix, i_min, j_min);
                //If (x_ij != 1) then devide all row elements on x_ij 
                RowDevision(_matrix, i_min, j_min);
                //Set 0-s in a col (by calculation way)
                SetNullsToColumn(_matrix, i_min, j_min);
            }
            //Trace.WriteLine("Результат");
            Service.TracingObjectMatrix(_matrix);
            Service.RecordMatrix(_matrix);
        }

        #endregion
        
        #region The general steps of working
        //A function which looks for a number of col of element. This element is a minimum (look for the first minimum)
        private static int FindMinimumElColumn(object[,] _matrix) 
        {
            int min_j = 2; //let x1 is a number of col which includes a minimum element
            //Don't consider artificial variables, because they are equaled "0" or "1"
            for (int j = 2; j < _matrix.GetLength(1) - FindArtificialVariablesCount(_matrix); j++)
                if (Convert.ToDouble(_matrix[_matrix.GetLength(0) - 1, j]) < Convert.ToDouble(_matrix[_matrix.GetLength(0) - 1, min_j]))
                    min_j = j;
            
            return min_j;
        }

        //Look for the first number of nonnegative quotient which introduces in a view of Value/Element Of Lead Col
        private static int FindNumberMinPosDivision(object[,] _matrix, int _min_j, int _subtractValue)
        {
            //_subtractValue = -1 or -2, because we don't  consider a target and/or artificial function
            for (int i = 1; i < _matrix.GetLength(0) - _subtractValue; i++)
            {
                if ((Convert.ToDouble(_matrix[i, _min_j]) > 0) && (Convert.ToDouble(_matrix[i, 1]) / Convert.ToDouble(_matrix[i, _min_j]) > 0))
                {
                    return i;
                }
            }
            return -1;
        }

        //Function looks for a number of lead row
        private static int FindNumberLeadRow(object[,] _matrix, int _min_j, int _subtractValue)
        {
            int min_i = FindNumberMinPosDivision(_matrix, _min_j, _subtractValue);
            double minDivisionRes = Convert.ToDouble(_matrix[min_i, 1]) / Convert.ToDouble(_matrix[min_i, _min_j]);
            
            //Work with artificial function
            if (_subtractValue == 2)
            {
                bool f_priority = false; //flag-priority between art and fect

                if (_matrix[min_i, 0].ToString().Contains("art"))
                    f_priority = true; //the priority of replacement "art" is higher
                
                //_subtractValue = -1 or -2, because we don't condiser a target and/or artificial function
                for (int i = 1; i < _matrix.GetLength(0) - _subtractValue; i++)
                {
                    if (_matrix[i, 0].ToString().Contains("art"))
                        f_priority = true; //the priority of replacement "art" is higher
                    else
                        f_priority = false;

                    if ((Convert.ToDouble(_matrix[i, _min_j]) > 0) && (Convert.ToDouble(_matrix[i, 1]) / Convert.ToDouble(_matrix[i, _min_j]) < minDivisionRes) && (Convert.ToDouble(_matrix[i, 1]) / Convert.ToDouble(_matrix[i, _min_j]) > 0))
                    {
                        minDivisionRes = (Convert.ToDouble(_matrix[i, 1])) / (Convert.ToDouble(_matrix[i, _min_j]));
                        min_i = i;
                    }
                    else
                    {
                        if ((Convert.ToDouble(_matrix[i, _min_j]) > 0) && (Convert.ToDouble(_matrix[i, 1]) / Convert.ToDouble(_matrix[i, _min_j]) == minDivisionRes) && (Convert.ToDouble(_matrix[i, 1]) / Convert.ToDouble(_matrix[i, _min_j]) > 0) && f_priority)
                        {
                            minDivisionRes = (Convert.ToDouble(_matrix[i, 1])) / (Convert.ToDouble(_matrix[i, _min_j]));
                            min_i = i;
                        }
                    }
                }
            }
            //in this case, work with target function
            else
            {
                //_subtractValue = -1 or -2, because we don't consider a target and/or artificial function
                for (int i = 1; i < _matrix.GetLength(0) - _subtractValue; i++)
                {
                    if ((Convert.ToDouble(_matrix[i, _min_j]) > 0) && (Convert.ToDouble(_matrix[i, 1]) / Convert.ToDouble(_matrix[i, _min_j]) < minDivisionRes) && (Convert.ToDouble(_matrix[i, 1]) / Convert.ToDouble(_matrix[i, _min_j]) > 0))
                    {
                        minDivisionRes = (Convert.ToDouble(_matrix[i, 1])) / (Convert.ToDouble(_matrix[i, _min_j]));
                        min_i = i;
                    }
                }
            }
            return min_i;
        }

        //Function which replaces a base
        private static void ChangeBase(object[,] _matrix, int _min_i, int _min_j)
        {
            //Trace.WriteLine("БЫЛ " + _matrix[_min_i, 0]);
            _matrix[_min_i, 0] = _matrix[0, _min_j];
            //Trace.WriteLine("СТАЛ " + _matrix[_min_i, 0]);
        }
        
        //Bring to basis (to 1)
        private static void RowDevision(object[,] _matrix, int _min_i, int _min_j)
        {
            if (Convert.ToDouble(_matrix[_min_i, _min_j]) != 1)
            {
                double devisor = Convert.ToDouble(_matrix[_min_i, _min_j]);
                //j = 1, because col (j = 0) is a base
                for (int j = 1; j < _matrix.GetLength(1); j++)
                    _matrix[_min_i, j] = Convert.ToDouble(_matrix[_min_i, j]) / devisor;       
            }
        }

        //Get a 0-s in a col
        private static void SetNullsToColumn(object[,] _matrix, int _min_i, int _min_j)
        {
            //i = 1, т.к. i = 0 - title of the table
            for (int i = 1; i < _matrix.GetLength(0); i++)
            {
                if (i != _min_i)
                {
                    double tmp = Convert.ToDouble(_matrix[i, _min_j]);
                    if (tmp != 0)
                    {
                        //run across a row elements, j = 1, because j = 0 col is a base
                        for (int j = 1; j < _matrix.GetLength(1); j++)
                            _matrix[i, j] = Convert.ToDouble(_matrix[i, j]) - tmp * Convert.ToDouble(_matrix[_min_i, j]);
                    }
                    else
                        continue;
                }
            }

        }                     
        
        #endregion
       
        #endregion
    }
}
