using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AresAdditiveDevicesPlugin.Extensions
{
    public static class TypeExtension
    {
        public static bool IsImmediateInstantiable(this Type type)
        {
            return !type.IsAbstract && !type.IsInterface;
        }

        public static bool IsInstantiable(this Type type)
        {
            if (type.IsImmediateInstantiable())
            {
                return true;
            }

            return type.FindInstantiableSubType() != null;
        }

        public static Type FindInstantiableSubType(this Type type)
        {
            if (IsImmediateInstantiable(type))
            {
                return type;
            }
            var solutionAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var compatibleAssembly = solutionAssemblies.FirstOrDefault(assembly =>
             assembly.DefinedTypes.FirstOrDefault(assemblyType =>
              !assemblyType.IsAbstract && !assemblyType.IsInterface && type.IsAssignableFrom(assemblyType))
             != null);

            return compatibleAssembly?.DefinedTypes.FirstOrDefault(assemblyType =>
             !assemblyType.IsAbstract && !assemblyType.IsInterface && type.IsAssignableFrom(assemblyType));
        }

        public static object GenerateInstance(this Type type)
        {
            try
            {
                var instantiableType = type.FindInstantiableSubType();
                object result = null;
                try
                {
                    result = Activator.CreateInstance(instantiableType);
                    return result;
                }
                catch (Exception)
                {
                    // ignore...

                }
                var instantiableConstructor = type.GetInstantiableConstructor();

                if (instantiableType.IsEnum)
                {
                    return instantiableType.GetEnumValues();
                }
                var instantiableConstructorParameters = instantiableConstructor?.GetParameters();
                var args = GenerateDefaultParameters(instantiableConstructorParameters);
                return GenerateInstance(instantiableType, args as object[]);
            }
            catch (Exception)
            {
            }
            return "";
        }

        public static IEnumerable<object> GenerateDefaultParameters(params ParameterInfo[] parameterInfos)
        {
            try
            {
                var instantiations = new List<object>();

                if (parameterInfos == null || parameterInfos.Length == 0)
                {
                    return instantiations;
                }

                foreach (var parameterInfo in parameterInfos)
                {
                    var instantiation = parameterInfo.ParameterType.GenerateInstance();
                    instantiations.Add(instantiation);
                }
                return instantiations;
            }
            catch (Exception)
            {
            }
            return new object[] { "" };
        }

        public static object GenerateInstance(this Type type, params object[] args)
        {
            try
            {
                return Activator.CreateInstance(type, args);
            }
            catch (Exception)
            {
                return GetSpecialCaseValue(type);
            }
        }

        private static object GetSpecialCaseValue(Type type)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }
            if (type.IsPointer)
            {
                return GeneratePointerValue(type);
            }
            if (type.IsByRef)
            {
                return GenerateReferenceValue(type);
            }
            return null;
        }

        private static object GeneratePointerValue(Type type)
        {
            var pointedTypeName = type.FullName.Remove(type.FullName.LastIndexOf(""));
            var pointedType = Type.GetType(pointedTypeName);
            if (pointedType == typeof(char))
            {
                return string.Empty;
            }

            return null;
        }

        private static object GenerateReferenceValue(Type type)
        {
            return string.Empty;
        }


        public static ConstructorInfo GetInstantiableConstructor(this Type type)
        {
            var constructorInfos = type.GetConstructors();
            foreach (var constructorInfo in constructorInfos)
            {
                var constructorParams = constructorInfo.GetParameters();
                if (constructorParams.Length == 0)
                {
                    return constructorInfo;
                }

                foreach (var constructorParam in constructorParams)
                {
                    if (!constructorParam.ParameterType.IsInstantiable())
                    {
                        break;
                    }
                }

                return constructorInfo;


            }
            return null;
        }
    }
}
