namespace LibSysmelSharp
{
    public class MetaclassType : BehaviorType
    {
        public ClassType? thisClass;

        public MetaclassType(SourcePosition? sourcePosition, BehaviorType? superclass, FieldDefinition[] fields) :
            base(sourcePosition, superclass, fields)
        {
        }

        public override bool IsMetaclassType()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return Intrinsics.GetSingleton().MetaclassClassType;
        }

        public override string ToString()
        {
            if (thisClass != null)
                return thisClass.ToString() + " class";
            return "a Metaclass";
        }
    }
}
