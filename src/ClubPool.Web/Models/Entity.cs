using System;
using System.ComponentModel.DataAnnotations;

// heavily based on SharpArchitecture's Entity classes
namespace ClubPool.Web.Models
{
  public class Entity
  {
    public int Id { get; protected set; }

    public bool IsTransient() {
      return Id.Equals(default(int));
    }

    public override bool Equals(object obj) {
      var other = obj as Entity;

      if (ReferenceEquals(this, other))
        return true;

      if (null == other || !GetType().Equals(other.GetRealType()))
        return false;

      if (HasSameNonDefaultIdAs(other))
        return true;

      return false;
    }

    public override int GetHashCode() {
      if (IsTransient()) {
        return base.GetHashCode();
      }
      else {
        unchecked {
          int hashCode = GetType().GetHashCode();
          return (hashCode * 31) ^ Id.GetHashCode();
        }
      }
    }

    protected virtual Type GetRealType() {
      return GetType();
    }
  
    private bool HasSameNonDefaultIdAs(Entity other) {
      return !IsTransient() &&
            !other.IsTransient() &&
            Id.Equals(other.Id);
    }
  }

  public class VersionedEntity : Entity
  {
    [Timestamp]
    [ConcurrencyCheck]
    public Byte[] Version { get; protected set; }

    [NotMapped]
    public string EncodedVersion { get { return null == Version ? null : Convert.ToBase64String(Version); } }
  }
}