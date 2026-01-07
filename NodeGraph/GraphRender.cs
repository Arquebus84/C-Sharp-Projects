using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphRender : MonoBehaviour
{
    public Transform[] vertex;
    public Vector2[] edges;

    public Graph<Transform> graph;

    public void GraphTest()
    {
        /**
         *              A
         *            / | \
         *           B--C  D
         *            \   /  
         *              E-----F
         */
        //graph = new Graph<Char>();
        //Char v1 = 'A', v2 = 'B', v3 = 'C', v4 = 'D', v5 = 'E', v6 = 'F';

        //graph.addVertex(v1); graph.addVertex(v2); graph.addVertex(v3); graph.addVertex(v4); graph.addVertex(v5); graph.addVertex(v6);

        //graph.addEdge(v1, v2); graph.addEdge(v1, v3); graph.addEdge(v1, v4);
        //graph.addEdge(v2, v3); graph.addEdge(v2, v5);
        //graph.addEdge(v4, v5);
        //graph.addEdge(v5, v6);
        ////graph.displayGraph();

        //for (int i = 0; i < graph.getVertexList().Count; i++)
        //{
        //    Debug.Log(graph.getEdgeList().GetValueOrDefault(i));
        //}
    }

    public void Start()
    {
        
    }

    public void OnDrawGizmos()
    {
        Init();

        if (vertex.Length == 0) { return; }

        foreach ((Transform v, LinkedList<Transform> list) in graph.getAdjList())
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(v.position, 0.5f);
            foreach (Transform adj in list)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(v.position, adj.position);
            }
        }
    }

    public void Init()
    {
        graph = new Graph<Transform>();

        for (int i = 0; i < vertex.Length; i++)
        {
            graph.addVertex(vertex[i]);
        }

        try
        {
            for (int i = 0; i < edges.Length; i++)
            {
                if ((Int32)edges[i].x >= edges.Length || (Int32)edges[i].y >= edges.Length) { break; }

                graph.addEdge(graph.getEdgeList().GetValueOrDefault((Int32)edges[i].x).transform,
                    graph.getEdgeList().GetValueOrDefault((Int32)edges[i].y).transform);

                //graph.addEdge(graph.getEdgeList().GetValueOrDefault(i), graph.getEdgeList().GetValueOrDefault(i));
            }
        }
        catch(Exception ex)
        {
            Debug.Log("Invalid connection");
        }
    }
}
