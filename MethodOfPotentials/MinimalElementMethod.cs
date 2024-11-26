namespace Lab4DO
{
	public static class MinimalElementMethod
	{
		public static double[,] BuildWayValueMatrix(double[,] startMatrix)
		{
			int n = startMatrix.GetLength(0);
			int m = startMatrix.GetLength(1);
			double[,] wayValueMatrix = new double[n - 1, m - 1];
			var foundMinimals = new List<(int row, int col)>();
			while (!IsWayValueMatrixFounded(startMatrix, n, m))
			{
				(int row, int col) minimalElement =
			   FindMinimalElement(startMatrix, foundMinimals, n, m);
				if (minimalElement == (-1, -1))
				{
					Console.WriteLine("Minimal element not found!");
					break;
				}
				foundMinimals.Add(minimalElement);
				double minimalValue = Math.Min(startMatrix[minimalElement.row, m
			   - 1],
				startMatrix[n - 1, minimalElement.col]);
				wayValueMatrix[minimalElement.row, minimalElement.col] =
					minimalValue;
				double remainder =
					Math.Max(startMatrix[minimalElement.row, m - 1],
					startMatrix[n - 1, minimalElement.col]) -
					minimalValue;
				(int row, int col) remainderIndex =
				   startMatrix[minimalElement.row, m - 1].Equals(minimalValue)
					? (n - 1, minimalElement.col)
					: (minimalElement.row, m - 1);
				(int row, int col) closeIndex = startMatrix[minimalElement.row,
			   m - 1].Equals(minimalValue)
				? (minimalElement.row, m - 1)
				: (n - 1, minimalElement.col);
				startMatrix[closeIndex.row, closeIndex.col] -= minimalValue;
				startMatrix[remainderIndex.row, remainderIndex.col] = remainder;
			}
			return wayValueMatrix;
		}
		private static (int row, int col) FindMinimalElement(double[,]
	   startMatrix,
		List<(int row, int col)> foundMinimals, int n, int m)
		{
			(int row, int col) minimalElementIndexes = (-1, -1);
			double minimalElementValue = double.MaxValue;
			for (int i = 0; i < n - 1; i++)
			{
				for (int j = 0; j < m - 1; j++)
				{
					if (startMatrix[i, j] < minimalElementValue &&
				   !foundMinimals.Contains((i, j)) &&
					(startMatrix[n - 1, j] != 0 || startMatrix[i, m - 1] !=
				   0))
					{
						minimalElementValue = startMatrix[i, j];
						minimalElementIndexes = (i, j);
					}
				}
			}

			return minimalElementIndexes;
		}
		private static bool IsWayValueMatrixFounded(double[,] startMatrix, int
	   n, int m)
		{
			double demandSum = 0;
			for (int j = 0; j < m - 1; j++)
			{
				demandSum += startMatrix[n - 1, j];
			}
			return demandSum == 0;
		}
	}
}
