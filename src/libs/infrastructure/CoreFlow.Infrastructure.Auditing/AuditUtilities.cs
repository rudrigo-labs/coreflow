using CoreFlow.Infrastructure.Auditing.Attributes;

namespace CoreFlow.Infrastructure.Auditing
{
    internal static class AuditUtilities
    {
        public static bool IsAuditDisabled(Type type) {
            var customAttributes = type.GetCustomAttributes(false);

            foreach (var customAttribute in customAttributes) {
                if (customAttribute.GetType() == typeof(NotAuditableAttribute)) {
                    var auditableAttribute = (NotAuditableAttribute)customAttribute;
                    return auditableAttribute.Enabled;
                }
            }

            return false;
        }

        public static bool IsAuditDisabled(Type type, string propertyName) 
        {
            try
            {
                if (propertyName == "Discriminator") //definir a propriedade de sombra discriminadora como não auditável
                    return false;

                if (type.GetProperty(propertyName) is not null)
                {                   

                    var customAttributes = type.GetProperty(propertyName).GetCustomAttributes(false);

                    foreach (var customAttribute in customAttributes)
                    {
                        if (customAttribute.GetType() == typeof(NotAuditableAttribute))
                        {
                            var auditableAttribute = (NotAuditableAttribute)customAttribute;
                            return auditableAttribute.Enabled;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                var x = ex;
                throw;
            }
            

            return false;
        }
    }
}
