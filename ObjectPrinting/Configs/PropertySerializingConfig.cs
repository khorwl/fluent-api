using System;
using System.Reflection;

namespace ObjectPrinting.Configs
{
    public class PropertySerializingConfig<TOwner, TPropType> : IPropertySerializingConfig<TOwner>
    {
        private readonly PrintingConfig<TOwner> parentConfig;
        private Func<TPropType, string> serializer;
        private readonly MemberInfo targetMember;

        public PropertySerializingConfig(PrintingConfig<TOwner> parent) : this(parent, null)
        {
        }

        public PropertySerializingConfig(PrintingConfig<TOwner> parent, MemberInfo targetMember)
        {
            parentConfig = parent;
            this.targetMember = targetMember;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> serializer)
        {
            this.serializer = serializer;

            if (targetMember == null)
                parentConfig.AddPropertySerializingConfig<TPropType>(this);
            else
                parentConfig.AddPropertySerializingConfig(this, targetMember);

            return parentConfig;
        }

        PrintingConfig<TOwner> IPropertySerializingConfig<TOwner>.ParentConfig => parentConfig;

        Func<object, string> IPropertySerializingConfig<TOwner>.Serializer => BuildSerializer();

        private Func<object, string> BuildSerializer()
        {
            return
                ob =>
                {
                    if (ob is TPropType property)
                        return serializer(property);

                    throw new ArgumentException($"Expected type {typeof(TPropType)}, but got {ob.GetType()} instead");
                };
        }
    }
}