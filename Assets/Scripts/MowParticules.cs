using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Random = System.Random;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class MowParticules : MonoBehaviour
{
    [SerializeField] private float maxParticules = 100;
    [SerializeField] float particuleLifeTime = 2;
    [SerializeField] private float spawnRatePerSeconds = 0.1f;
    [SerializeField] private float initialSpeed = 0.5f;
    [SerializeField] private float initialUpVelocity = 1.0f;
    [SerializeField] private float gravity = -.98f;
    [SerializeField] private float lossOfMomentum = 0.99f;
    [SerializeField] private float minZ = 0.6f;
    private Material material;
    private LawnMower lawnMower;
    private Mesh quad;
    private List<Matrix4x4> particules;
    private List<Vector4> colors;
    private List<Vector4> velocities;
    private List<float> timeToLive;
    private GameManager gameManager;
    MaterialPropertyBlock colorsProperty;
    private Random rnd;

    private void Start()
    {
        rnd = new Random();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        lawnMower = GetComponent<LawnMower>();

        material = new Material(Shader.Find("Custom/ParticulesShader"));
        material.enableInstancing = true;

        quad = GeometryUtils.CreateQuad(1.0f / gameManager.Map.PixelsPerUnits, 1.0f / gameManager.Map.PixelsPerUnits);
        gameManager.OnGameStart += OnGameStart;
        particules = new List<Matrix4x4>();
        velocities = new List<Vector4>();
        timeToLive = new List<float>();
        colors = new List<Vector4>();
        colorsProperty = new MaterialPropertyBlock();
    }

    private void OnGameStart()
    {
        StartCoroutine(SpawnRoutine());
    }

    public void OnDestroy()
    {
        gameManager.OnGameStart -= OnGameStart;
    }

    private void Update()
    {
        for (int i = particules.Count - 1; i >= 0; i--)
        {
            timeToLive[i] -= Time.deltaTime;
            if (particules[i].m23 > minZ || timeToLive[i] <= 0)
            {
                particules.RemoveAt(i);
                timeToLive.RemoveAt(i);
                velocities.RemoveAt(i);
                colors.RemoveAt(i);
            }
            else
            {
                velocities[i] = new Vector4(
                    velocities[i].x * lossOfMomentum,
                    velocities[i].y * lossOfMomentum,
                    velocities[i].z - (gravity * Time.deltaTime),
                    1);
                Vector4 newPosition = new Vector4(
                    particules[i].m03,
                    particules[i].m13,
                    particules[i].m23,
                    particules[i].m33);

                newPosition.x += velocities[i].x * Time.deltaTime;
                newPosition.y += velocities[i].y * Time.deltaTime;
                newPosition.z += velocities[i].z * Time.deltaTime;
                particules[i] = new Matrix4x4(
                    new Vector4(1, 0, 0, 0),
                    new Vector4(0, 1, 0, 0),
                    new Vector4(0, 0, 1, 0),
                    newPosition);
            }

            if (lawnMower.Mowing && particules.Count != 0)
            {
                colorsProperty = new MaterialPropertyBlock();
                colorsProperty.SetVectorArray("_Colors", colors);
                Graphics.DrawMeshInstanced(quad, 0, material, particules.ToArray(), particules.Count, colorsProperty);
            }
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (lawnMower.Mowing)
        {
            yield return new WaitForSeconds(spawnRatePerSeconds);
            if (particules.Count < maxParticules)
            {
                particules.Insert(0, Matrix4x4.Translate(new Vector3(transform.position.x, transform.position.y, .5f)));
                velocities.Insert(0, new Vector4(
                    ((rnd.Next(-50, 50) / 100f) * 2) + (-transform.up.x * initialSpeed),
                    (rnd.Next(-50, 50) / 100f) + (-transform.up.y * initialSpeed),
                    -initialUpVelocity,
                    0));
                timeToLive.Insert(0, particuleLifeTime);
                Color color;
                if (lawnMower.IsStuck)
                {
                    color = gameManager.Map.SampleMap(lawnMower.transform.position);
                }
                else
                {
                    Vector3 front = lawnMower.transform.position + lawnMower.transform.up * 0.5f;

                    color = gameManager.Map.SampleMap(
                        new Vector3(front.x + rnd.Next(-50, 50) / 200f,
                            front.y + rnd.Next(-50, 50) / 200f,
                            front.z));
                }

                colors.Insert(0, color);
            }
        }
    }
}