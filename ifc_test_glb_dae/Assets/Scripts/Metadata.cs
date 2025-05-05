using System;
using System.Collections.Generic;

[Serializable]
public class Metadata
{
    public long ExpressID;
    public string GlobalId;
    public string Class;
    public string PredefinedType;
    public string Name;
    public string Level;
    public string ObjectType;
    public Dictionary<string, object> QuantitySets;
    public Dictionary<string, object> PropertySets;
}
