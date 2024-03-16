#if FIRST_PERSON_CONTROLLER && THIRD_PERSON_CONTROLLER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateCharacterController.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CBG {

    public class UCCFPSMaterialController : FPSMaterialController {

#if UNITY_EDITOR

        protected override void FindInvisibleMaterial() {
            var perspectiveMonitor = GetComponent<Opsive.UltimateCharacterController.ThirdPersonController.Character.PerspectiveMonitor>();
            if (perspectiveMonitor) {
                invisibleMaterial = perspectiveMonitor.InvisibleMaterial;
            } else {
                base.FindInvisibleMaterial();
            }
        }

#endif

        private void OnEnable() {
            EventHandler.RegisterEvent<bool>(gameObject, "OnCharacterChangePerspectives", OnPerspectiveChanged);
        }

        private void OnDisable() {
            EventHandler.UnregisterEvent<bool>(gameObject, "OnCharacterChangePerspectives", OnPerspectiveChanged);
        }

    }
}
#endif