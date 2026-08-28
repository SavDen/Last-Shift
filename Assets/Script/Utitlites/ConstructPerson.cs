using System.Collections.Generic;
using UnityEngine;

public class ConstructPerson : MonoBehaviour
{
    public enum Gender
    {
        male,
        female,
    }
    
    [SerializeField] private List<Material> _materials = new();
    
    [SerializeField] List<GameObject> _maleBody, _famaleBody;
    
    [SerializeField] List<Mesh> _maleHair, _famaleHair;
}
