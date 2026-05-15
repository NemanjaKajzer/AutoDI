using System;
using System.Collections.Immutable;

namespace AutoDI.SourceGenerator
{
    internal sealed class RegistrationModel : IEquatable<RegistrationModel>
    {
        public string ImplementationFQN { get; }
        public ImmutableArray<string> InterfaceFQNs { get; }
        public ServiceLifetime Lifetime { get; }

        public RegistrationModel(
            string implementationFQN,
            ImmutableArray<string> interfaceFQNs,
            ServiceLifetime lifetime)
        {
            ImplementationFQN = implementationFQN;
            InterfaceFQNs = interfaceFQNs;
            Lifetime = lifetime;

        }
        public bool Equals(RegistrationModel other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (ImplementationFQN != other.ImplementationFQN) return false;
            if (Lifetime != other.Lifetime) return false;
            if (InterfaceFQNs.Length != other.InterfaceFQNs.Length) return false;

            for (int i = 0; i < InterfaceFQNs.Length; i++)
            {
                if (InterfaceFQNs[i] != other.InterfaceFQNs[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
            => Equals(obj as RegistrationModel);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (ImplementationFQN?.GetHashCode() ?? 0);
                hash = hash * 31 + Lifetime.GetHashCode();
                foreach (var iface in InterfaceFQNs)
                    hash = hash * 31 + (iface?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
