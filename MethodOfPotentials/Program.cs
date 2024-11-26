using Lab4DO;
public abstract class Program
{
	public static void Main(string[] args)
	{
		double[,] startMatrix =
		{
		 { 2, 2, 5, 7, 170 },
		 { 5, 6.1, 2, 3, 129 },
		 { 4, 4, 3, 6.2, 115 },
		 { 8, 2, 2, 7, 240 },
		 { 117, 140, 310, 87, 654 }
		};
		MethodOfPotentials.FindSolution(startMatrix);
	}
}