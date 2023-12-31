﻿namespace Exceptions;

public class xException : Exception { }

public class AxeQException : xException { public override string Message => "∀x∈ℚ"; }
public class xeOException : xException { public override string Message => "x∈∅"; }
public class UnsolvableException : xException { public override string Message => "Unsolvable"; }

public class DivideByPolynomialException : Exception { public override string Message => "Divisor cannot be a polynomial"; }