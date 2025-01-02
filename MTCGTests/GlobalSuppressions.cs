// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// suppress NUnit2005 globally for the entire project
[assembly: SuppressMessage(
    "Assertion",
    "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)",
    Justification = "Consistency with existing tests and team conventions")]

