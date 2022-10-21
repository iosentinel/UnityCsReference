// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
    [ExcludeFromDocs]
    public struct AwaitableCoroutineAsyncMethodBuilder<T>
    {
        private interface IStateMachineBox : IDisposable
        {
            AwaitableCoroutine<T> ResultingCoroutine { get; set; }
            Action MoveNext { get; }
        }
        private class StateMachineBox<TStateMachine> : IStateMachineBox where TStateMachine : IAsyncStateMachine
        {
            static ThreadSafeObjectPool<StateMachineBox<TStateMachine>> _pool = new(() => new());
            public static StateMachineBox<TStateMachine> GetOne() { return _pool.Get(); }
            public void Dispose()
            {
                StateMachine = default;
                ResultingCoroutine = null;
                _pool.Release(this);
            }
            public TStateMachine StateMachine { get; set; }

            public Action MoveNext { get; }

            private void DoMoveNext()
            {
                StateMachine.MoveNext();
            }

            public StateMachineBox()
            {
                MoveNext = DoMoveNext;
            }

            public AwaitableCoroutine<T> ResultingCoroutine { get; set; }
        }
        IStateMachineBox _stateMachineBox;

        IStateMachineBox EnsureStateMachineBox<TStateMachine>() where TStateMachine : IAsyncStateMachine
        {
            if(_stateMachineBox != null)
            {
                return _stateMachineBox;
            }
            _stateMachineBox = StateMachineBox<TStateMachine>.GetOne();
            _stateMachineBox.ResultingCoroutine = _resultingCoroutine;
            return _stateMachineBox;
        }
        public static AwaitableCoroutineAsyncMethodBuilder<T> Create() => default;
        AwaitableCoroutine<T> _resultingCoroutine;
        public AwaitableCoroutine<T> Task {
            get
            {
                if (_resultingCoroutine != null)
                {
                    return _resultingCoroutine;
                }
                if (_stateMachineBox != null)
                {
                    return _resultingCoroutine = _stateMachineBox.ResultingCoroutine ??= AwaitableCoroutine<T>.GetManaged();
                }
                return _resultingCoroutine = AwaitableCoroutine<T>.GetManaged();
            }
        }

        public void SetResult(T value)
        {
            Task.SetResultAndRaiseContinuation(value);
            _stateMachineBox.Dispose();
            _stateMachineBox = null;
        }
        public void SetException(Exception e)
        {
            Task.SetExceptionAndRaiseContinuation(e);
            _stateMachineBox.Dispose();
            _stateMachineBox = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            var box = EnsureStateMachineBox<TStateMachine>();
            ((StateMachineBox<TStateMachine>)box).StateMachine = stateMachine;
            stateMachine.MoveNext();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
    where TAwaiter : INotifyCompletion
    where TStateMachine : IAsyncStateMachine
        {
            var box = EnsureStateMachineBox<TStateMachine>();
            ((StateMachineBox<TStateMachine>)box).StateMachine = stateMachine;
            awaiter.OnCompleted(box.MoveNext);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            var box = EnsureStateMachineBox<TStateMachine>();
            ((StateMachineBox<TStateMachine>)box).StateMachine = stateMachine;
            awaiter.UnsafeOnCompleted(box.MoveNext);
        }
    }
}
