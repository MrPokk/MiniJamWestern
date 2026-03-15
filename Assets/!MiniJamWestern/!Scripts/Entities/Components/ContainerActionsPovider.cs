

using System;
using System.Collections.Generic;
using BitterECS.Integration.Unity;

[Serializable]
public class ContainerActions
{
    public List<TagActionsProvider> listAction;
}

public class ContainerActionsProvider : ProviderEcs<ContainerActions> { }

