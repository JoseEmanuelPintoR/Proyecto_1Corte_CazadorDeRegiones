using UnityEngine;

public static class LimitesCamara
{

    public static float MitadAlto(Camera camara, float planoZ)
    {
        if (camara == null)
        {
            return 0f;
        }

        if (camara.orthographic)
        {
            return camara.orthographicSize;
        }

        float profundidad = planoZ - camara.transform.position.z;

        if (profundidad <= 0f)
        {
            return 0f;
        }

        return profundidad * Mathf.Tan(camara.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }

    public static float MitadAncho(Camera camara, float planoZ)
    {
        if (camara == null)
        {
            return 0f;
        }

        return MitadAlto(camara, planoZ) * camara.aspect;
    }
}
