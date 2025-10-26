using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine.AI;

public class tempjob : MonoBehaviour
{
    public GameObject _prefab;
    public List<NavMeshAgent> _agents;
    public bool UseJobs;
    // Start is called before the first frame update

    public void changeBool()
    {
        UseJobs = !UseJobs;
    }

    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject go=(Instantiate(_prefab, new Vector3(UnityEngine.Random.Range(-20, 20), 0.5f, UnityEngine.Random.Range(-20, 20)), quaternion.identity));
            _agents.Add(go.GetComponent<NavMeshAgent>());
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        if (!UseJobs)
        {
            for (int i = 0; i < _agents.Count; i++)
            {

                Vector3 _newPosition = new Vector3(UnityEngine.Random.Range(-20, 20), 0.5f, UnityEngine.Random.Range(-20, 20));
                _agents[i].SetDestination(_newPosition);
                float s = 0;
                for (int temp = 0; temp < 5000000; temp++)
                {
                    s += i + temp;
                }
            }
        }
        else
        {
            NativeArray<float3> _TP = new NativeArray<float3>(_agents.Count,Allocator.TempJob);
            paralleljob _parallelJob = new paralleljob
            {
                _positionToGo = _TP
            };
            JobHandle _JH = _parallelJob.Schedule(_agents.Count, 100);
            _JH.Complete();

            for (int i = 0; i < _agents.Count; i++)
            {
                _agents[i].SetDestination(_TP[i]);

            }
            _TP.Dispose();
        }

    }




}


[BurstCompile]
public struct paralleljob : IJobParallelFor
{
    public NativeArray<float3> _positionToGo;
    private Unity.Mathematics.Random _random ;
    private float i;

    public void Execute(int index)
    {
        i = 0;
        _random = new Unity.Mathematics.Random(1);
        float x = _random.NextInt(-20, 20);
        float y = _random.NextInt(-20, 20);

        _positionToGo[index] = new float3(x, 0.5f, y);
        float s = 0;
        for (int temp = 0; temp < 5000000; temp++)
        {
            s += i + temp;
        }
    }
}
