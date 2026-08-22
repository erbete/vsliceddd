using System;

namespace Domain.Common;

public sealed class InvariantViolationException(string message) : Exception($"Invariant violated: {message}");