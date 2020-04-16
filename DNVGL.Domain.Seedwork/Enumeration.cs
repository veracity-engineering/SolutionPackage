using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DNVGL.Domain.Seedwork
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Enumeration<T> : IComparable where T: IComparable
    {
        public T Id { get; }

        public string Name { get; }

        protected Enumeration(T id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;

        public static IEnumerable<TEnumeration> GetAll<TEnumeration>() where TEnumeration : Enumeration<T>
        {
            var fields = typeof(TEnumeration).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<TEnumeration>();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Enumeration<T> otherValue))
                return false;

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Id.GetHashCode();

//        public static int AbsoluteDifference(Enumeration<T> firstValue, Enumeration<T> secondValue)
//        {
//            var absoluteDifference = Math.Abs(firstValue.Id.CompareTo(secondValue.Id));
//            return absoluteDifference;
//        }

        public static TEnumeration FromValue<TEnumeration>(T value) where TEnumeration : Enumeration<T>
        {
            var matchingItem = Parse<TEnumeration, T>(value, "value", item => item.Id.Equals(value));
            return matchingItem;
        }

        public static TEnumeration FromDisplayName<TEnumeration>(string displayName) where TEnumeration : Enumeration<T>
        {
            var matchingItem = Parse<TEnumeration, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        private static TEnumeration Parse<TEnumeration, TU>(TU value, string description, Func<TEnumeration, bool> predicate) where TEnumeration : Enumeration<T>
        {
            var matchingItem = GetAll<TEnumeration>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration<T>)other).Id);
    }
}
