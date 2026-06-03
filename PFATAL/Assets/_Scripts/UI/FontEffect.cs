using UnityEngine;
using TMPro;

// copyright : eloulou
public class FontEffect : MonoBehaviour
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Vector2 _wobblePower;
    [SerializeField] private float _wobbleSpeed;
    [SerializeField] private float _colorSpeed = 0.5f;
    [SerializeField] private float _colorSpread = 0.1f;

    private TMP_Text _textMesh;
    private Mesh _mesh;
    private Vector3[] _vertices;

    void Start()
    {
        _textMesh = TryGetComponent(out TMP_Text textMesh) ? textMesh : null;
    }

    void Update()
    {
        _textMesh.ForceMeshUpdate();
        _mesh = _textMesh.mesh;
        _vertices = _mesh.vertices;

        Vector3[] baseVertices = (Vector3[])_vertices.Clone();
        Color[] colors = _mesh.colors;

        for (int i = 0; i < _textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = _textMesh.textInfo.characterInfo[i];

            if (!c.isVisible) continue;

            int index = c.vertexIndex;

            Vector3 offset = Wobble((Time.time + i) * _wobbleSpeed);

            colors[index]     = _gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed + i * _colorSpread, 1f));
            colors[index + 1] = _gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed + i * _colorSpread, 1f));
            colors[index + 2] = _gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed + i * _colorSpread, 1f));
            colors[index + 3] = _gradient.Evaluate(Mathf.Repeat(Time.time * _colorSpeed + i * _colorSpread, 1f));

            _vertices[index]     = baseVertices[index]     + offset;
            _vertices[index + 1] = baseVertices[index + 1] + offset;
            _vertices[index + 2] = baseVertices[index + 2] + offset;
            _vertices[index + 3] = baseVertices[index + 3] + offset;
        }

        _mesh.vertices = _vertices;
        _mesh.colors = colors;
        _textMesh.canvasRenderer.SetMesh(_mesh);
    }

    Vector3 Wobble(float time)
    {
        return new Vector3(Mathf.Sin(time) * _wobblePower.x, Mathf.Sin(time + 1.5f) * _wobblePower.y, 0f);
    }
}