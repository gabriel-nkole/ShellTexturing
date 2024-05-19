using UnityEngine;
using System;

public class SimpleShell : MonoBehaviour{

    [SerializeField]
    Mesh ShellMesh;

    String lastShellMeshName;

    [SerializeField]
    Shader ShellShader;

    [SerializeField, Range(1, 256)]
    uint ShellCount;

    uint lastShellCount;

    [SerializeField, Range(0, 5)]
    float MaxHeight;

    [SerializeField, Range(1, 1000)]
    int Density;

    [SerializeField, Range(0, 1)]
    float NoiseMin;

    [SerializeField, Range(0, 1)]
    float NoiseMax;

    [SerializeField, Range(0f, 20f)]
    float Thickness;

    [SerializeField, Range(0f, 10f)]
    float Curvature;

    [SerializeField, Range(0f, 1f)]
    float DisplacementStrength;

    [SerializeField]
    Color ShellColor;

    float scaleUniv = 20f;
    float scaleQuad = 1f;
    float scalePlane = 0.1f;
    float scale3D = 1f/3f;

    float movementSpeed = 0.1f;

    Vector3 D = Vector3.zero;

    bool hasChildren;


    void Awake() {
        lastShellCount = ShellCount;
        lastShellMeshName = ShellMesh.name;

        scaleQuad *= scaleUniv;
        scalePlane *= scaleUniv;
        scale3D *= scaleUniv;
    }

    void OnEnable(){
        String ShellMeshName = ShellMesh.name;
        Vector3 localScale = Vector3.one * scale3D;
        Quaternion rotation = Quaternion.identity;
        if (ShellMeshName == "Quad") {
            localScale = new Vector3(scaleQuad, scaleQuad, 1f);
            rotation = Quaternion.Euler(90, 0, 0);
        }
        else if (ShellMeshName == "Plane") {
            localScale = new Vector3(scalePlane, 1f, scalePlane);
            rotation = Quaternion.Euler(0, -180, 0);
        }

        Material ShellMaterial = new Material(ShellShader);
        ShellMaterial.SetFloat("_maxHeight", MaxHeight);
        ShellMaterial.SetInt("_density", Density);
        ShellMaterial.SetFloat("_noiseMin", NoiseMin);
        ShellMaterial.SetFloat("_noiseMax", NoiseMax);
        ShellMaterial.SetFloat("_thickness", Thickness);
        ShellMaterial.SetFloat("_curvature", Curvature);
        ShellMaterial.SetFloat("_dispStrength", DisplacementStrength);
        ShellMaterial.SetColor("_shellColor", ShellColor);

        for (uint i = 0; i < ShellCount; i++) {
            float h = 0;
            if (ShellCount != 1) {
                h = ((float)i / (float)(ShellCount - 1));
            }
            
            GameObject Shell = new GameObject(System.String.Format("Shell {0}", i));
            Shell.AddComponent<MeshFilter>().mesh = ShellMesh;
            Shell.AddComponent<MeshRenderer>().material = ShellMaterial;

            Shell.transform.SetParent(this.transform);
            Shell.transform.localScale = localScale;
            Shell.transform.rotation = rotation;
            
            Shell.GetComponent<Renderer>().material.SetFloat("_h", h);
        }
        hasChildren = true;
    }

    void OnDisable() {
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(this.transform.GetChild(i).gameObject);
        }
        hasChildren = false;
    }

    void Update(){
        if (hasChildren == true && enabled) {
            if (ShellCount != lastShellCount || ShellMesh.name != lastShellMeshName) {
                OnDisable();
                OnEnable();
            }
            lastShellCount = ShellCount;
            lastShellMeshName = ShellMesh.name;

            Vector3 M = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f) * movementSpeed;
            //Vector3 D = new Vector3(0f, -1f, 0f);
            D = Vector3.Normalize(D - M);
            for (int i = 0; i < ShellCount; i++) {
                GameObject Shell = this.transform.GetChild(i).gameObject;
                Vector3 prevPosition = Shell.transform.position;
                Shell.transform.position = prevPosition + M;

                Material ShellMaterial = Shell.GetComponent<MeshRenderer>().material;
                ShellMaterial.SetFloat("_maxHeight", MaxHeight);
                ShellMaterial.SetInt("_density", Density);
                ShellMaterial.SetFloat("_noiseMin", NoiseMin);
                ShellMaterial.SetFloat("_noiseMax", NoiseMax);
                ShellMaterial.SetFloat("_thickness", Thickness);
                ShellMaterial.SetVector("_D", D);
                ShellMaterial.SetFloat("_curvature", Curvature);
                ShellMaterial.SetFloat("_dispStrength", DisplacementStrength);
                ShellMaterial.SetColor("_shellColor", ShellColor);
            }
        }
    }
}
