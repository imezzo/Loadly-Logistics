namespace LoadlyAggregator.Domain.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class NoAuditAttribute : Attribute;
