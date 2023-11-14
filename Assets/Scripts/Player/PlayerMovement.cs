using System.Collections;
using System.Collections.Generic;
using HS4.UI;
using Unity.Netcode;
using UnityEngine;

namespace HS4.PlayerCore
{
    public struct InputPayload : INetworkSerializable
    {
        public int Tick;
        public Vector2 Move;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref Move);
        }
    }

    public struct StatePayload : INetworkSerializable
    {
        public int Tick;
        public Vector3 Position;
        public Quaternion Rotation;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Rotation);
        }
    }

    public class PlayerMovement : NetworkBehaviour
    {
       
        [Header("Refs")]
        [SerializeField] private CharacterController _controller;
        [SerializeField] private PlayerAnimation _animation;
        [SerializeField] private PlayerView _playerView;

        [Header("Stats")]
        [SerializeField] private GameObject _footstepOb;
        [SerializeField] private float _speed = 1f;
        public float SpeedChangeRate = 10.0f;
        public float RotationSmoothTime = 12f;
        private float _targetRotation = 0.0f;
        private float _verticalVelocity = -9.18f;
        private bool _footStep = false;
        [SerializeField] private GameObject test;
        [SerializeField] private GameObject Test2;
        //Netcode
        NetworkTimer _timer;
        const float k_serverTickRate = 60f;
        const int k_bufferSize = 1024;

        //netcode client
        CircularBuffer<StatePayload> _clientStateBuffer;
        CircularBuffer<InputPayload> _clientInputBuffer;
        InputPayload _lastInputPayload;
        StatePayload _lastServerState;
        StatePayload _lastProcessState;

        //netcode server
        CircularBuffer<StatePayload> _serverStateBuffer;
        Queue<InputPayload> _serverInputQueue;

        [SerializeField] private NetworkVariable<bool> _isCanMove = new NetworkVariable<bool>(false);
        public bool _isStartGame;
      
        // Start is called before the first frame update
        void Awake()
        {
            _timer = new NetworkTimer(k_serverTickRate);
            _clientInputBuffer = new CircularBuffer<InputPayload>(k_bufferSize);
            _clientStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);

            _serverStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);
            _serverInputQueue = new Queue<InputPayload>();

            // Test2 =Instantiate(test);
        }

        public void EnableInput() {
            _isStartGame = true;
            if(IsOwner) UIController.Instance.Active();
        }

        public void SetCanMove(bool isKill) {
            if(IsServer)
             _isCanMove.Value = !isKill;
        } 

        public void ResetPlayer() {
            _isStartGame = false;
        }
        private void FixedUpdate()
        {
            _timer.Update(Time.fixedDeltaTime);
           // Test2.transform.position = this.transform.position;
            if(!_isCanMove.Value || !_isStartGame ) 
                return;

            while (_timer.ShouldTick())
            {
                HandleClientTick();
                HandleServerTick();
            }
        }

        private void HandleClientTick()
        {

            if (IsOwner)
            {
                var currentTick = _timer.CurrentTick;
                var bufferIndex = currentTick % k_bufferSize;

                InputPayload inputPayload = new InputPayload()
                {
                    Tick = currentTick,
                    Move = new Vector2(Input.GetAxis("Horizontal") + UIController.Instance.MoveJoyStick.Horizontal
                                    , Input.GetAxis("Vertical") + UIController.Instance.MoveJoyStick.Vertical)
                };

                _clientInputBuffer.Add(inputPayload, bufferIndex);
                SendToServerRpc(inputPayload);

                if (!IsHost)
                {
                    StatePayload statePayload = ProcessMove(inputPayload);
                    _clientStateBuffer.Add(statePayload, bufferIndex);
                }

            }
            else if(IsClient && !IsServer)
            {
               ProcessMove(_lastInputPayload);
            }

        }
        private void HandleServerTick()
        {
            if (!IsServer) return;

            var bufferIndex = -1;
            InputPayload inputPayload = default;
            while (_serverInputQueue.Count > 0)
            {
                inputPayload = _serverInputQueue.Dequeue();
                bufferIndex = inputPayload.Tick % k_bufferSize;

                StatePayload statePayload = ProcessMove(inputPayload);
                _serverStateBuffer.Add(statePayload, bufferIndex);
            }

            if (bufferIndex == -1) return;
            SendToClientRpc(_serverStateBuffer.Get(bufferIndex), inputPayload);

        }
        [ClientRpc]
        private void SendToClientRpc(StatePayload statePayload, InputPayload inputPayload)
        {
            if (IsOwner)
            {
                _lastServerState = statePayload;
            }
            else
            {
                _lastInputPayload = inputPayload;
            }
        }
        [ServerRpc]
        void SendToServerRpc(InputPayload inputPayload)
        {
            _serverInputQueue.Enqueue(inputPayload);
        }

        private StatePayload ProcessMove(InputPayload inputPayload)
        {
            Move(inputPayload);

            return new StatePayload()
            {
                Tick = inputPayload.Tick,
                Position = transform.position,
                Rotation = transform.rotation
            };
        }

        private void Move(InputPayload inputPayload)
        {
            
            Vector3 inputDirection = new Vector3(inputPayload.Move.x, 0.0f, inputPayload.Move.y).normalized;

            if (inputPayload.Move != Vector2.zero)
            {

                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, _targetRotation, 0.0f), RotationSmoothTime * Time.fixedDeltaTime);

                Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
               
                _controller.Move(targetDirection.normalized * (_speed * Time.fixedDeltaTime) +
                            new Vector3(0.0f, 0, 0.0f) * Time.fixedDeltaTime);
                
                
               
                _animation.Walk();

                if(_footStep == false && _playerView.ObjectHide)
                {
                    StartCoroutine(InitFootstep());
                }
            }
            else {
                _animation.Idle();
                _footStep = false;
            }
                

        }


        IEnumerator InitFootstep() {
            _footStep = true;
            yield return new WaitForSeconds(3f);
            GameObject footstep = Instantiate(_footstepOb, this.transform.position,Quaternion.identity);
            footstep.GetComponent<ParticleSystem>().Play();
            footstep.transform.position = new Vector3(transform.position.x, 0.5902482f, transform.position.z);
            footstep.transform.eulerAngles = new Vector3(90f,transform.eulerAngles.y,0);
            Destroy(footstep,2f);
            _footStep = false;
        }


    }

}
