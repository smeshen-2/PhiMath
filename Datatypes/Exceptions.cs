namespace Exceptions;

public class xException : Exception { }

public class AllRealRootsException : xException { public override string Message => "∀x∈ℝ"; }
public class NoRootsException : xException { public override string Message => "x∉ℝ"; }
public class NoRealRootsException : NoRootsException { }
public class NoRationalRootsException : NoRootsException { public override string Message => "Unsolvable"; }

public class DivideByPolynomialException : Exception { public override string Message => "Division yields polynomial denominator"; }