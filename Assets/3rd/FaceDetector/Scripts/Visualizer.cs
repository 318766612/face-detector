using UnityEngine;
using UnityEngine.UI;
using Klak.TestTools;
using MediaPipe.BlazeFace;

public sealed class Visualizer : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] ImageSource _source = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField, Range(0, 1)] float _threshold = 0.75f;
    [SerializeField] RawImage _previewUI = null;
    [SerializeField] Marker _markerPrefab = null;

    #endregion

    #region Private members

    FaceDetector _detector;
    Marker[] _markers = new Marker[16];

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // Face detector initialization
        _detector = new FaceDetector(_resources);

        // Marker population
        for (var i = 0; i < _markers.Length; i++)
            _markers[i] = Instantiate(_markerPrefab, _previewUI.transform);
    }

    void OnDestroy()
    {
        _detector?.Dispose();
    }

    void LateUpdate()
    {
        // Face detection
        _detector.ProcessImage(_source.Texture, _threshold);
        //// Marker update
        var i = 0;
        //////Debug.Log(_detector.Detections.Length);
        foreach (var detection in _detector.Detections)
        {
            if (i == _markers.Length) break;
            var marker = _markers[i++];
            marker.detection = detection;
            marker.gameObject.SetActive(true);
        }

        for (; i < _markers.Length; i++)
            _markers[i].gameObject.SetActive(false);

        // UI update
        _previewUI.texture = _source.Texture;

        //Texture2D tex = new Texture2D(_source.OutputResolution.x, _source.OutputResolution.y, TextureFormat.ARGB32, false);
        //RenderTexture.active = (RenderTexture)_previewUI.texture;
        //tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
        //tex.Apply(false, false);

        //MoleGameMgr.Instance.moleImage = tex.EncodeToJPG();
        ////Debug.Log(MoleGameMgr.Instance.moleImage.Length);
        //RenderTexture.active = null;
        //Texture2D.Destroy(tex);
        //tex = null;
        //MoleGameMgr.moleImage = tex.EncodeToJPG();
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //SavePNG();
        //}
    }

    public byte[] SavePNG()
    {
        if (_previewUI != null)
        {
            Texture2D tex = new Texture2D(_source.OutputResolution.x, _source.OutputResolution.y, TextureFormat.ARGB32, false);
            RenderTexture.active = (RenderTexture)_previewUI.texture;
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
            tex.Apply(false, false);

            byte[] data = tex.EncodeToJPG();
            //Debug.Log(MoleGameMgr.Instance.moleImage.Length);
            //string date = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            ////string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
            //string imgPath = System.IO.Directory.GetParent(Application.dataPath) + "/GLXS_" + date + "_" + _source.OutputResolution.x + "x" + _source.OutputResolution.y + ".jpg";


            //Debug.Log("Save Image Path :" + savePath);
            //System.IO.File.WriteAllBytes(savePath, pngBytes);

            RenderTexture.active = null;
            Texture2D.Destroy(tex);
            tex = null;
            return data;
        }
        return null;
    }

    #endregion
}