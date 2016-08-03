using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Globalization;

namespace Com.EnjoyCodes.SqlHelper
{
    /// <summary>
    /// The PropertyAccessor is used to set or get the property value.
    /// </summary>
    /// <typeparam name="T">The type of target instance.</typeparam>
    /// <remarks>The underlying mechanism is based on IL Emit, which provides better performance than pure reflection.</remarks>
    public class PropertyAccessor
    {
        #region Private Static Fields
        private static Dictionary<PropertyAccessorKey, PropertyAccessor> propertyAccessors = new Dictionary<PropertyAccessorKey, PropertyAccessor>();
        private static object synchHelper = new object();
        private static IDictionary<Type, OpCode> valueTpyeOpCodes;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the type of the target.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType { get; private set; }
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the getter <see cref="MethodInfo"/> of the property.
        /// </summary>
        /// <value>The getter <see cref="MethodInfo"/>.</value>
        public MethodInfo GetMethod { get; private set; }

        /// <summary>
        /// Gets the setter <see cref="MethodInfo"/> of the property.
        /// </summary>
        /// <value>The setter <see cref="MethodInfo"/>.</value>
        public MethodInfo SetMethod { get; private set; }


        /// <summary>
        /// Gets or sets the delegate to get property value.
        /// </summary>
        /// <value>The delegate to get property value.</value>
        public Func<object, object> GetFunction { get; private set; }

        /// <summary>
        /// Gets or sets the delegate to set property value.
        /// </summary>
        /// <value>The delegate to set property value.</value>
        public Action<object, object> SetAction { get; private set; }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes the <see cref="PropertyAccessor&lt;T&gt;"/> class.
        /// </summary>
        static PropertyAccessor()
        {
            valueTpyeOpCodes = new Dictionary<Type, OpCode>();
            valueTpyeOpCodes[typeof(sbyte)] = OpCodes.Ldind_I1;
            valueTpyeOpCodes[typeof(byte)] = OpCodes.Ldind_U1;
            valueTpyeOpCodes[typeof(char)] = OpCodes.Ldind_U2;
            valueTpyeOpCodes[typeof(short)] = OpCodes.Ldind_I2;
            valueTpyeOpCodes[typeof(ushort)] = OpCodes.Ldind_U2;
            valueTpyeOpCodes[typeof(int)] = OpCodes.Ldind_I4;
            valueTpyeOpCodes[typeof(uint)] = OpCodes.Ldind_U4;
            valueTpyeOpCodes[typeof(long)] = OpCodes.Ldind_I8;
            valueTpyeOpCodes[typeof(ulong)] = OpCodes.Ldind_I8;
            valueTpyeOpCodes[typeof(bool)] = OpCodes.Ldind_I1;
            valueTpyeOpCodes[typeof(double)] = OpCodes.Ldind_R8;
            valueTpyeOpCodes[typeof(float)] = OpCodes.Ldind_R4;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAccessor&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyAccessor(Type targetType, string propertyName)
        {
            Guard.ArgumentNotNullOrEmpty(targetType, "targetType");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            this.PropertyName = propertyName;
            this.TargetType = targetType;
            this.GetMethod = this.TargetType.GetMethod(string.Format("get_{0}", propertyName));
            this.SetMethod = this.TargetType.GetMethod(string.Format("set_{0}", propertyName));
            this.GetFunction = CreateGetFunction();
            this.SetAction = CreateSetAction();
        }
        #endregion

        #region Public Instance Methods

        /// <summary>
        /// Gets the property value of the given object.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <returns>The property value of the given object.</returns>
        public object Get(object obj)
        {
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            EnsureValidType(obj, "obj");
            return this.GetFunction(obj);
        }



        /// <summary>
        /// Sets the property value of the given object.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <param name="value">The property value.</param>
        public void Set(object obj, object value)
        {
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            EnsureValidType(obj, "obj");
            this.SetAction(obj, value);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Gets the property value of the given object.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The property value of the given object.</returns>
        public static object Get(object obj, string propertyName)
        {
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            PropertyAccessor propertyAccessor;
            PropertyAccessorKey key = new PropertyAccessorKey(obj.GetType(), propertyName);
            if (propertyAccessors.ContainsKey(key))
            {
                propertyAccessor = propertyAccessors[key];
                return propertyAccessor.Get(obj);
            }
            lock (synchHelper)
            {
                if (propertyAccessors.ContainsKey(key))
                {
                    propertyAccessor = propertyAccessors[key];
                    return propertyAccessor.Get(obj);
                }
                propertyAccessor = new PropertyAccessor(obj.GetType(), propertyName);
                propertyAccessors[key] = propertyAccessor;
            }
            return propertyAccessor.Get(obj);
        }

        /// <summary>
        /// Sets the property value of the given object.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The property value.</param>
        public static void Set(object obj, string propertyName, object value)
        {
            Guard.ArgumentNotNullOrEmpty(obj, "obj");
            Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
            PropertyAccessor propertyAccessor;
            PropertyAccessorKey key = new PropertyAccessorKey(obj.GetType(), propertyName);
            if (propertyAccessors.ContainsKey(key))
            {
                propertyAccessor = propertyAccessors[key];
                propertyAccessor.Set(obj, value);
                return;
            }
            lock (synchHelper)
            {
                if (propertyAccessors.ContainsKey(key))
                {
                    propertyAccessor = propertyAccessors[key];
                    propertyAccessor.Set(obj, value);
                    return;
                }
                propertyAccessor = new PropertyAccessor(obj.GetType(), propertyName);
                propertyAccessors[key] = propertyAccessor;
            }
            propertyAccessor.Set(obj, value);
        }

        #endregion

        #region Private Methods

        private void EnsureValidType(object value, string parameterName)
        {
            if (!this.TargetType.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException("The target type cannot be assignable from the type of given object.", parameterName);
            }
        }

        private Func<object, object> CreateGetFunction()
        {
            if (null == this.GetMethod)
            {
                throw new InvalidOperationException(
                   string.Format("Cannot find a readable \"{0}\" from the type \"{1}\".", this.PropertyName, this.TargetType.FullName));
            }

            DynamicMethod method = new DynamicMethod("GetValue", typeof(object), new Type[] { typeof(object) });
            ILGenerator ilGenerator = method.GetILGenerator();
            ilGenerator.DeclareLocal(typeof(object));
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Castclass, this.TargetType);
            ilGenerator.EmitCall(OpCodes.Call, this.GetMethod, null);
            if (this.GetMethod.ReturnType.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, this.GetMethod.ReturnType);
            }
            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);

            method.DefineParameter(1, ParameterAttributes.In, "value");
            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }

        private Action<object, object> CreateSetAction()
        {
            if (null == this.SetMethod)
            {
                throw new InvalidOperationException(
                    string.Format("Cannot find a writable \"{0}\" from the type \"{1}\".", this.PropertyName, this.TargetType.FullName));
            }
            DynamicMethod method = new DynamicMethod("SetValue", null, new Type[] { typeof(object), typeof(object) });
            ILGenerator ilGenerator = method.GetILGenerator();
            Type paramType = this.SetMethod.GetParameters()[0].ParameterType;
            ilGenerator.DeclareLocal(paramType);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Castclass, this.TargetType);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            if (paramType.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox, paramType);
                if (valueTpyeOpCodes.ContainsKey(paramType))
                {
                    OpCode load = (OpCode)valueTpyeOpCodes[paramType];
                    ilGenerator.Emit(load);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Ldobj, paramType);
                }
            }
            else
            {
                ilGenerator.Emit(OpCodes.Castclass, paramType);
            }

            ilGenerator.EmitCall(OpCodes.Callvirt, this.SetMethod, null);
            ilGenerator.Emit(OpCodes.Ret);

            method.DefineParameter(1, ParameterAttributes.In, "obj");
            method.DefineParameter(2, ParameterAttributes.In, "value");
            return (Action<object, object>)method.CreateDelegate(typeof(Action<object, object>));
        }
        #endregion
    }
}