using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace BerserkPixel.Prata
{
    public class DialogInteractor : MonoBehaviour
    {
        [SerializeField] private InteractionMode mode = InteractionMode.Mode2D;
        [SerializeField] private float detectionRadius;
        [SerializeField] private Transform origin;
        [SerializeField] private LayerMask targetMask;

        [SerializeField] private float timeForReset = 2f;

        public UnityEvent<Interaction> OnDetection;
        public UnityEvent OnCancelled;
        public UnityEvent<Interaction> OnInteractionReset;
        public UnityEvent OnNoMoreDialog;

        private Interaction _interaction;

        private IInteractionHolder _interactionHolder;

        private bool _dialogEnabled;

        private Coroutine _resetDialog;

        private void LateUpdate()
        {
            var hit = GetHit(mode);

            if (hit.IsValid)
            {
                // if there's someone to interact with
                if (hit.hitTransform.TryGetComponent(out _interactionHolder))
                {
                    _interaction = _interactionHolder.GetInteraction();

                    if (_interaction != null && _interaction.HasAnyDialogLeft())
                    {
                        if (!_dialogEnabled)
                        {
                            _dialogEnabled = true;
                            CancelResetDialog();
                            OnDetection?.Invoke(_interaction);
                        }
                    }
                    else if (_interaction != null && !_interaction.HasAnyDialogLeft())
                    {
                        if (_dialogEnabled)
                        {
                            _dialogEnabled = false;
                            OnNoMoreDialog?.Invoke();
                            _resetDialog = StartCoroutine(ResetDialog());
                        }
                    }
                }
            }
            else
            {
                // if there's no one near to interact with

                // if we have previously started a dialog, 
                if (_dialogEnabled)
                {
                    _dialogEnabled = false;

                    _interaction?.Cancel();
                    _interactionHolder?.Reset();
                    DialogManager.Instance.HideDialog();

                    OnCancelled?.Invoke();

                    CancelResetDialog();

                    _resetDialog = StartCoroutine(ResetDialog());
                }
            }
        }

        private void CancelResetDialog()
        {
            if (_resetDialog != null)
            {
                StopCoroutine(_resetDialog);
                _resetDialog = null;
            }
        }

        private IEnumerator ResetDialog()
        {
            yield return new WaitForSeconds(timeForReset);

            _interactionHolder?.Reset();
            _interactionHolder = null;

            if (_interaction != null)
            {
                _interaction.Reset();
                OnInteractionReset?.Invoke(_interaction);
                _interaction = null;
            }
        }

        private InteractionHit GetHit(InteractionMode mode)
        {
            if (mode.Equals(InteractionMode.Mode2D))
            {
                Collider2D hit = Physics2D.OverlapCircle(origin.position, detectionRadius, targetMask);
                if (hit)
                {
                    return new InteractionHit(hit.transform);
                }
                else
                {
                    return new InteractionHit(null);
                }

            }
            else if (mode.Equals(InteractionMode.Mode3D))
            {
                int maxColliders = 3;
                Collider[] hitColliders = new Collider[maxColliders];
                int numColliders = Physics.OverlapSphereNonAlloc(origin.position, detectionRadius, hitColliders, targetMask);

                if (numColliders > 0)
                {
                    return new InteractionHit(hitColliders[0].transform);
                }
                else
                {
                    return new InteractionHit(null);
                }
            }
            else
            {
                throw new MissingReferenceException("The Interaction mode is not valid. Please choose a valid one.");
            }
        }

        private void OnDrawGizmos()
        {
            if (origin != null)
            {
                Gizmos.color = new Color(0, 0, 1, 0.1f);
                Gizmos.DrawSphere(origin.position, detectionRadius);
            }
        }
    }
}
