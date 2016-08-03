using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace ReflectionTest
{
    public class MemberComparer<ObjectType> : IComparer<ObjectType>
    {
        private delegate int CompareDelegate(ObjectType x, ObjectType y);
        private CompareDelegate _compare;

        public MemberComparer(string memberName)
        {
            _compare = GetCompareDelegate(memberName);
        }

        public int Compare(ObjectType x, ObjectType y)
        {
            return _compare(x, y);
        }

        private CompareDelegate GetCompareDelegate(string memberName)
        {
            Type objectType = typeof(ObjectType);

            PropertyInfo pi = objectType.GetProperty(memberName);
            FieldInfo fi = objectType.GetField(memberName);
            Type memberType = null;
            bool isProperty = false;

            if (pi != null)
            {
                if (pi.GetGetMethod() != null)
                {
                    memberType = pi.PropertyType;
                    isProperty = true;
                }
                else
                    throw new Exception(String.Format(
                        "Property: '{0}' of Type: '{1}' does not have a Public Get accessor",
                        memberName, objectType.Name));
            }
            else if (fi != null)
                memberType = fi.FieldType;
            else
                throw new Exception(String.Format(
                    "Property: '{0}' of Type: '{1}' does not have a Public Get accessor",
                    memberName, objectType.Name));

            Type comparerType = typeof(Comparer<>).MakeGenericType(new Type[] { memberType });
            MethodInfo getDefaultMethod = comparerType.GetProperty("Default").GetGetMethod();
            MethodInfo compareMethod = getDefaultMethod.ReturnType.GetMethod("Compare");

            DynamicMethod dm = new DynamicMethod("Compare_" + memberName, typeof(int), new Type[] { objectType, objectType }, comparerType);
            ILGenerator il = dm.GetILGenerator();

            // Load Comparer<memberType>.Default onto the stack
            il.EmitCall(OpCodes.Call, getDefaultMethod, null);

            // Load the member from arg 0 onto the stack
            il.Emit(OpCodes.Ldarg_0);
            if (isProperty)
                il.EmitCall(OpCodes.Callvirt, pi.GetGetMethod(), null);
            else
                il.Emit(OpCodes.Ldfld);

            // Load the member from arg 1 onto the stack
            il.Emit(OpCodes.Ldarg_1);
            if (isProperty)
                il.EmitCall(OpCodes.Callvirt, pi.GetGetMethod(), null);
            else
                il.Emit(OpCodes.Ldfld);

            // Call the Compare method
            il.EmitCall(OpCodes.Callvirt, compareMethod, null);

            il.Emit(OpCodes.Ret);

            return (CompareDelegate)dm.CreateDelegate(typeof(CompareDelegate));
        }
    }
}
