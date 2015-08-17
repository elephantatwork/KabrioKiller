#pragma strict

//Static class with extra functions
public static class F
{
	//Same as Mathf.Pow, but only excepts natural numbers (positive integers) as the exponent
	//It might be a little faster than Mathf.Pow
	public function PowNatural(n : float, exp : int) : float
	{
		var result : float = n;
		exp = Mathf.Max(exp, 0);

		if (exp == 0)
		{
			result = 1;
		}
		else if (exp == 2)
		{
			result = n * n;
		}
		else if (exp > 2)
		{
			for (var i : int = 0; i < exp; i++)
			{
				if (i > 0)
				{
					result *= n;
				}
			}
		}

		return result;
	}

	//Returns the number with the greatest absolute value
	//There are multiple versions for varying numbers of parameters, only 1-3 are supported here
	public function MaxAbs(n : float)
	{
		return n;
	}
	
	public function MaxAbs(n0 : float, n1 : float)
	{
		var result : float;
		
		if (Mathf.Abs(n0) > Mathf.Abs(n1))
		{
			result = n0;
		}
		else
		{
			result = n1;
		}
		
		return result;
	}
	
	public function MaxAbs(n0 : float, n1 : float, n2 : float)
	{
		var result : float;
		
		if (Mathf.Abs(n0) > Mathf.Abs(n1) && Mathf.Abs(n0) > Mathf.Abs(n2))
		{
			result = n0;
		}
		else if (Mathf.Abs(n1) > Mathf.Abs(n0) && Mathf.Abs(n1) > Mathf.Abs(n2))
		{
			result = n1;
		}
		else
		{
			result = n2;
		}
		
		return result;
	}
}
