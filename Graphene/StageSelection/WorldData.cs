using System;
using System.Collections.Generic;
using System.Linq;
using Graphene.UiGenerics;
using UnityEngine;
using UnityEngine.UI;

namespace StageSelection
{
    public enum StageType
    {
        Tutorial = 0,
        Dungeon = 1,
        Rest = 2,
        Bonus = 3
    }

    [Serializable]
    public class StageData
    {
        public string LevelName;
        public int Id;
        public int Level;
        public int Traps;
        public int Sacrifices;
        public int Recovers;
        public int Element;
        public StageType Type;
        public int[] Unlock;
        public float Duration;
    }

    class IdPoint
    {
        public int id;
        public Transform point;
    }

    public class WorldData : CanvasGroupView
    {
        public List<StageData> StageData;

        public Vector2 Separation;

        public Sprite[] Elements;

        public string StagePrefab = "WorldStage";
        private GameObject _objWorldStage;

        private List<IdPoint> _ids;

        private int dir;

        Transform _last;

        public void GeneratePoints()
        {
            DeleteChildren();
            var rect = transform as RectTransform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, StageData.Count * Separation.y);
            _objWorldStage = Resources.Load<GameObject>(StagePrefab);
            var stages = StageData.OrderBy(x => x.Id).ToList();

            _ids = new List<IdPoint>();
            dir = 0;
            DrawStage(stages[0], 1);

            FindObjectOfType<Scrollbar>().value = 0;
        }

        private void DeleteChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        private void DrawStage(StageData data, int height)
        {
            var k = _ids.FindIndex(x => x.id == data.Id);
            data.Level = height - 1;

            Transform tmpt;
            if (k < 0)
            {
                var tmp = Instantiate(_objWorldStage, transform);
                tmp.transform.localPosition = new Vector3(Separation.x * dir, Separation.y * height, 0);
                var stg = tmp.GetComponent<IStageSelector>();
                stg.SetStage(data);
                tmpt = tmp.transform;
                _ids.Add(new IdPoint() {id = data.Id, point = tmpt});
            }
            else
            {
                tmpt = _ids[k].point;
            }

            if (_last != null)
            {
                var line = tmpt.GetComponentInChildren<LineRenderer>();
                
                if (k >= 0)
                {
                    line = Instantiate(line, line.transform.parent);
                    line.transform.localPosition = Vector3.zero;
                }
                
                line.useWorldSpace = false;
                var ldir = tmpt.position - _last.transform.position;
                var pos = new Vector3[]
                {
                    -ldir * Scale,
                    Vector3.zero - ldir * Offset,
                };
                line.SetPositions(pos);
            }
            else
            {
                var line = tmpt.GetComponentInChildren<LineRenderer>();
                line.gameObject.SetActive(false);
            }

            if (data.Type == StageType.Rest)
            {
                tmpt.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                tmpt.GetChild(0).GetComponent<Image>().sprite = Elements[data.Element];
            }

            int i = 0, n = data.Unlock.Length - 1;
            
            foreach (var unlock in data.Unlock)
            {
                dir = (int) (-n / 2f) + i;
                _last = tmpt;
                DrawStage(StageData.Find(x => x.Id == unlock), height + 1);
                i++;
            }
        }

        public float Scale = 10, Offset = 1;
    }
}