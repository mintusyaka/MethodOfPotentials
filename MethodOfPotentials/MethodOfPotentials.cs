using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab4DO;
public static class MethodOfPotentials
{
	public static void FindSolution(double[,] startMatrix)
	{
		Console.WriteLine("Start matrix:");
		PrintMatrix(startMatrix);
		double[,] wayValueMatrix =
	   MinimalElementMethod.BuildWayValueMatrix(startMatrix);
		Console.WriteLine("\nWays cost matrix:");
		double Z = 0;
		for (int i = 0; i < wayValueMatrix.GetLength(0); i++)
		{
			for (int j = 0; j < wayValueMatrix.GetLength(1); j++)
			{
				Z += wayValueMatrix[i, j] * startMatrix[i, j];
			}
		}

		Console.WriteLine($"Z = {Z}");
		do
		{
			Console.WriteLine();
			PrintMatrix(wayValueMatrix);
			Console.WriteLine();
			(double[] u, double[] v) potentials =
		   CalculatePotentials(startMatrix, wayValueMatrix);
			double[,] deltas = CalculateDeltas(startMatrix, wayValueMatrix,
		   potentials.u, potentials.v);
			(int row, int col) maxDelta = FindMaxDelta(deltas);
			if (deltas[maxDelta.row, maxDelta.col] <= 0)
			{
				Z = 0;
				for (int i = 0; i < wayValueMatrix.GetLength(0); i++)
				{
					for (int j = 0; j < wayValueMatrix.GetLength(1); j++)
					{
						Z += wayValueMatrix[i, j] * startMatrix[i, j];
					}
				}

				Console.WriteLine($"Z = {Z}");

				Console.WriteLine("ALGO END!");
				return;
			}
			List<(int row, int col)> cycle = FindCycle(wayValueMatrix,
		   maxDelta.row, maxDelta.col);
			cycle.Reverse();
			foreach (var node in cycle)
			{
				Console.Write($"({node.row},{node.col}) -> ");
			}
			Console.WriteLine();
			UpdateWayValueMatrix(wayValueMatrix, cycle);
		} while (true);
	}
	private static void UpdateWayValueMatrix(double[,] wayValueMatrix, List<(int
   row, int col)> cycle)
	{
		double minVal = Double.MaxValue;
		for (int i = 0; i < cycle.Count; i++)
		{
			if (i % 2 != 0 && minVal > wayValueMatrix[cycle[i].row,
		   cycle[i].col])
			{
				minVal = wayValueMatrix[cycle[i].row, cycle[i].col];
			}
		}
		for (int i = 0; i < cycle.Count; i++)
		{
			if (i % 2 == 0)
			{
				wayValueMatrix[cycle[i].row, cycle[i].col] += minVal;
			}
			else
			{
				wayValueMatrix[cycle[i].row, cycle[i].col] -= minVal;
			}
		}
	}
	private static List<(int row, int col)> FindCycle(double[,] wayValueMatrix,
   int rowOfMaxDelta, int colOfMaxDelta)
	{
		Stack<(int row, int col)> cycle = new Stack<(int row, int col)>();
		bool[,] isVisited = new bool[wayValueMatrix.GetLength(0),
	   wayValueMatrix.GetLength(1)];
		bool isHorizontalMove = true;
		cycle.Push((rowOfMaxDelta, colOfMaxDelta));
		(int row, int col) ver = (-1, -1);
		do
		{
			if (ver != (-1, -1) && ver != cycle.Peek())
			{
				cycle.Push(ver);
			}
			else if (cycle.Count > 1 && ver == (-1, -1))
			{
				cycle.Pop();
			}
			if (isHorizontalMove)
			{
				ver = SearchHorizontal(wayValueMatrix, isVisited,
			   cycle.Peek().row, cycle.Peek().col);
				isHorizontalMove = !isHorizontalMove;
			}
			else
			{
				ver = SearchVertical(wayValueMatrix, isVisited,
			   cycle.Peek().row, cycle.Peek().col);
				isHorizontalMove = !isHorizontalMove;
			}
		} while (ver.col != colOfMaxDelta);
		cycle.Push(ver);
		return cycle.ToList();
	}
	private static (int row, int col) SearchVertical(double[,] wayValueMatrix,
   bool[,] isVisited, int row,
	int col)
	{
		(int row, int col) nextVer = (-1, -1);
		//go up
		for (int i = row; i >= 0; i--)
		{
			if (wayValueMatrix[i, col] != 0 && !isVisited[i, col])
			{
				nextVer = (i, col);
				break;
			}
		}
		//go down
		if (nextVer == (-1, -1))
		{
			for (int i = row; i < wayValueMatrix.GetLength(0); i++)
			{
				if (wayValueMatrix[i, col] != 0 && !isVisited[i, col])
				{
					nextVer = (i, col);
					break;
				}
			}
		}
		if (nextVer != (-1, -1))
		{
			isVisited[nextVer.row, nextVer.col] = true;
		}
		return nextVer;
	}
	private static (int row, int col) SearchHorizontal(double[,] wayValueMatrix,
   bool[,] isVisited, int row,
	int col)
	{
		(int row, int col) nextVer = (-1, -1);
		//go to left
		for (int i = col; i >= 0; i--)
		{
			if (wayValueMatrix[row, i] != 0 && !isVisited[row, i])
			{
				nextVer = (row, i);
				break;
			}
		}
		//go to right
		if (nextVer == (-1, -1))
		{
			for (int i = col; i < wayValueMatrix.GetLength(1); i++)
			{
				if (wayValueMatrix[row, i] != 0 && !isVisited[row, i])
				{
					nextVer = (row, i);
					break;
				}
			}
		}
		if (nextVer != (-1, -1))
		{
			isVisited[nextVer.row, nextVer.col] = true;
		}
		return nextVer;
	}
	private static (double[] u, double[] v) CalculatePotentials(double[,]
   startMatrix, double[,] wayValueMatrix)
	{
		int m = wayValueMatrix.GetLength(0);
		int n = wayValueMatrix.GetLength(1);
		double[] u = new double[m];
		double[] v = new double[n];
		bool[] uAssigned = new bool[m];
		bool[] vAssigned = new bool[n];
		int indexOfFirstU =
	   FindIndexOfRowWhereMaxNumberOfBasesElements(wayValueMatrix);
		u[indexOfFirstU] = 0;
		uAssigned[indexOfFirstU] = true;
		bool updated = true;
		while (updated)
		{
			updated = false;
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					if (wayValueMatrix[i, j] > 0)
					{
						if (uAssigned[i] && !vAssigned[j])
						{
							v[j] = startMatrix[i, j] - u[i];
							vAssigned[j] = true;
							updated = true;
						}
						else if (!uAssigned[i] && vAssigned[j])
						{
							u[i] = startMatrix[i, j] - v[j];
							uAssigned[i] = true;
							updated = true;
						}
					}
				}
			}
		}
		return (u, v);
	}
	private static double[,] CalculateDeltas(double[,] startMatrix, double[,]
   wayValueMatrix, double[] u,
	double[] v)
	{
		int m = wayValueMatrix.GetLength(0);
		int n = wayValueMatrix.GetLength(1);
		double[,] deltaMatrix = new double[m, n];
		for (int i = 0; i < m; i++)
		{
			for (int j = 0; j < n; j++)
			{
				if (wayValueMatrix[i, j] == 0)
				{
					deltaMatrix[i, j] = u[i] + v[j] - startMatrix[i, j];
				}
				else
				{
					deltaMatrix[i, j] = 0;
				}
			}
		}
		return deltaMatrix;
	}
	private static (int row, int col) FindMaxDelta(double[,] deltas)
	{
		int n = deltas.GetLength(0);
		int m = deltas.GetLength(1);
		double maxDeltaValue = double.MinValue;
		(int row, int col) maxDeltaIndexes = (-1, -1);
		for (int i = 0; i < n; i++)
		{
			for (int j = 0; j < m; j++)
			{
				if (deltas[i, j] > maxDeltaValue)
				{
					maxDeltaValue = deltas[i, j];
					maxDeltaIndexes = (i, j);
				}
			}
		}
		return maxDeltaIndexes;
	}
	private static int FindIndexOfRowWhereMaxNumberOfBasesElements(double[,]
   wayValuesMatrix)
	{
		int maxBasesElementsInRow = int.MinValue;
		int indexOfRowWhereMaxNumberOfBasesElements = -1;
		int n = wayValuesMatrix.GetLength(0);
		int m = wayValuesMatrix.GetLength(1);
		for (int i = 0; i < n; i++)
		{
			int numOfBasesInRow = 0;
			for (int j = 0; j < m; j++)
			{
				if (wayValuesMatrix[i, j] != 0)
				{
					numOfBasesInRow++;
				}
			}
			if (maxBasesElementsInRow < numOfBasesInRow)
			{
				maxBasesElementsInRow = numOfBasesInRow;
				indexOfRowWhereMaxNumberOfBasesElements = i;
			}
		}
		return indexOfRowWhereMaxNumberOfBasesElements;
	}
	private static void PrintMatrix(double[,] matrix)
	{
		int rows = matrix.GetLength(0);
		int cols = matrix.GetLength(1);
		int maxLength = 0;
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				string number = matrix[i, j].ToString("0.##");
				if (number.Length > maxLength)
				{
					maxLength = number.Length;
				}
			}
		}
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				Console.Write(matrix[i, j].ToString("0.##").PadLeft(maxLength +
			   2) + " ");
			}
			Console.WriteLine();
		}
	}

}
