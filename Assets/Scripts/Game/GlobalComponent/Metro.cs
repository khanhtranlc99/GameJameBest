using Game.Character;
using Game.Character.CharacterController;
using Game.MiniMap;
using Game.Vehicle;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GlobalComponent
{
    public class Metro : MonoBehaviour
    {
        private const float MetroHight = -2f;

        public Transform EnterPoint;

        public CutsceneManager MainEnterCutsceneManager;

        public CutsceneManager MainExitCutsceneManager;

        public MetroMarkForMiniMap MetroMark;

        public Bounds DeathBox;

        public List<Rigidbody> m_ObstructiveRigids;

        private void Start()
        {
            MetroManager.Instance.RegisterMetro(this);
            if (!MetroMark)
            {
                MetroMark = GetComponentInChildren<MetroMarkForMiniMap>();
            }
        }

        public void EnterInMetro(CutsceneManager.Callback callback)
        {
            MetroManager.Instance.CurrentMetro = this;
            MainEnterCutsceneManager.Init(callback, ExitMetroCallback);
        }

        public void RemoveObstructive()
        {
            m_ObstructiveRigids = new List<Rigidbody>();
            Collider[] array = Physics.OverlapBox(base.transform.TransformPoint(DeathBox.center), DeathBox.extents, base.transform.rotation);
            for (int i = 0; i < array.Length; i++)
            {
                Rigidbody attachedRigidbody = array[i].attachedRigidbody;
                if (attachedRigidbody != null && !m_ObstructiveRigids.Contains(attachedRigidbody))
                {
                    m_ObstructiveRigids.Add(attachedRigidbody);
                }
            }
            for (int j = 0; j < m_ObstructiveRigids.Count; j++)
            {
                DrivableVehicle component = m_ObstructiveRigids[j].GetComponent<DrivableVehicle>();
                if (component != null)
                {
                    component.DestroyVehicle();
                }
            }
        }

        public void ExitFromMetro()
        {
            MetroManager.Instance.CurrentMetro = this;
            MetroManager.Instance.TerminusMetro = null;
            MetroPanel.Instance.CloseMetroPanel();
            var transform1 = PlayerInteractionsManager.Instance.Player.transform;
            transform1.position = EnterPoint.position;
            transform1.rotation = EnterPoint.rotation;
            MainExitCutsceneManager.Init(delegate
            {
                MetroManager.Instance.InMetro = false;
            }, ExitMetroCallback);
        }

        private void ExitMetroCallback()
        {
            MetroManager.Instance.InMetro = false;
        }

        private void OnTriggerEnter(Collider col)
        {
            Player componentInParent = col.GetComponentInParent<Player>();
            if (componentInParent != null && !componentInParent.IsFlying && !PlayerInteractionsManager.Instance.inVehicle)
            {
                Vector3 position = componentInParent.transform.position;
                float y = position.y;
                Vector3 position2 = transform.position;
                if (y - position2.y < -2f)
                {
                    MetroManager.Instance.GetInMetro();
                }
            }
        }
    }
}
