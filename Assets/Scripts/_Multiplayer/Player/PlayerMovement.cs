using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace HS4
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

        [Header("Stats")]
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _rotationSpeed = 180f;
        public float SpeedChangeRate = 10.0f;
        public float RotationSmoothTime = 12f;
        private float _targetRotation = 0.0f;
        private float _verticalVelocity = -9.18f;

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


         [SerializeField] private NetworkVariable<bool> _isCanMove = new NetworkVariable<bool>(true,
                                                                    NetworkVariableReadPermission.Everyone,
                                                                    NetworkVariableWritePermission.Server);
      
        // Start is called before the first frame update
        void Awake()
        {
            _timer = new NetworkTimer(k_serverTickRate);
            _clientInputBuffer = new CircularBuffer<InputPayload>(k_bufferSize);
            _clientStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);

            _serverStateBuffer = new CircularBuffer<StatePayload>(k_bufferSize);
            _serverInputQueue = new Queue<InputPayload>();
        }

        public void SetCanMove(bool isKill) => _isCanMove.Value = !isKill;

        void Update()
        {
            _timer.Update(Time.deltaTime);
        }
        private void FixedUpdate()
        {
            if(!_isCanMove.Value) 
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
                    Move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))
                };

                _clientInputBuffer.Add(inputPayload, bufferIndex);
                SendToServerRpc(inputPayload);

                if (!IsHost)
                {
                    StatePayload statePayload = ProcessMove(inputPayload);
                    _clientStateBuffer.Add(statePayload, bufferIndex);
                }

            }
            else
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

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, _targetRotation, 0.0f), RotationSmoothTime * Time.deltaTime);

                Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                new Vector3(0.0f, 0, 0.0f) * Time.deltaTime);
                _animation.Walk();
            }
            else
                _animation.Idle();

        }


    }

}
