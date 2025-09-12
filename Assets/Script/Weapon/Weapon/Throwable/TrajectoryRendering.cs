using UnityEngine;

public class TrajectoryRendering 
{
    private LineRenderer _lineRenderer;
    private float _force;
    private bool _activeRender;
    public Vector3 spawnOffset = new Vector3(0, 2f, 0.5f);

    public TrajectoryRendering(LineRenderer lineRenderer, float force)
    {
        _lineRenderer = lineRenderer;
        _force = force;
    }

    public void ActiveRender(bool state) => _activeRender = state;


    public void ShowTrRender(Transform throwPos)
    {
        if(_activeRender)
        {
            Vector3[] points = new Vector3[50];

            Vector3 startPos = throwPos.position + spawnOffset;

            Vector3 throwDirection = (throwPos.forward + Vector3.up * 0.5f).normalized;
            Vector3 velocity = throwDirection * _force;
            int countPoint = 0;

            for (int i = 0; i < 50; i++)
            {

                float time = i * 0.1f;
                points[i] = startPos + velocity * time + 0.5f * Physics.gravity * time * time;
                countPoint++;

                if (points[i].y < -0.5f) break;
            }

            _lineRenderer.SetPositions(points);
            _lineRenderer.positionCount = countPoint;
        }

        else
        {
            _lineRenderer.positionCount = 0;
        }
    }

}
