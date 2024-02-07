using UnityEngine;

public class CellVis : MonoBehaviour
{
	public int _age;
	public Vector3Int _position;
	
	private ConwayManager _manager;
    private MeshRenderer _renderer;
	
	public void Initialize(Vector3Int position, ConwayManager manager)
	{
		_position = position;
		_manager = manager;
		
		_renderer = GetComponent<MeshRenderer>();
		SetAgeColor();
	}
        
	public void Increment()
	{
		_age++;

		SetAgeColor();
	}
	
	private void SetAgeColor()
	{
		if(_age < _manager._cellMaterials.Count)
			_renderer.material = _manager._cellMaterials[_age];
	}
}