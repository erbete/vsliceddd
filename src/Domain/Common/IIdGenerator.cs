using System;

namespace Domain.Common;

public interface IIdGenerator
{
    Guid NewId();
}