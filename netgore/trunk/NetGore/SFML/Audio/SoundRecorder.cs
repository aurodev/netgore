using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace SFML
{
    namespace Audio
    {
        ////////////////////////////////////////////////////////////
        /// <summary>
        /// SoundRecorder is an interface for capturing sound data,
        /// it is meant to be used as a base class
        /// </summary>
        ////////////////////////////////////////////////////////////
        public abstract class SoundRecorder : ObjectBase
        {
            ////////////////////////////////////////////////////////////
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            delegate bool ProcessCallback(IntPtr samples, uint nbSamples, IntPtr userData);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            delegate bool StartCallback();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            delegate void StopCallback();

            readonly ProcessCallback myProcessCallback;
            readonly StartCallback myStartCallback;
            readonly StopCallback myStopCallback;

            #region Imports

            [DllImport("csfml-audio", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern bool sfSoundRecorder_CanCapture();

            [DllImport("csfml-audio", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern IntPtr sfSoundRecorder_Create(StartCallback OnStart, ProcessCallback OnProcess, StopCallback OnStop,
                                                        IntPtr UserData);

            [DllImport("csfml-audio", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern void sfSoundRecorder_Destroy(IntPtr SoundRecorder);

            [DllImport("csfml-audio", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern uint sfSoundRecorder_GetSampleRate(IntPtr SoundRecorder);

            [DllImport("csfml-audio", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern void sfSoundRecorder_Start(IntPtr SoundRecorder, uint SampleRate);

            [DllImport("csfml-audio", CallingConvention = CallingConvention.Cdecl)]
            [SuppressUnmanagedCodeSecurity]
            static extern void sfSoundRecorder_Stop(IntPtr SoundRecorder);

            #endregion

            /// <summary>
            /// Default constructor
            /// </summary>
            ////////////////////////////////////////////////////////////
            protected SoundRecorder() : base(IntPtr.Zero)
            {
                myStartCallback = new StartCallback(OnStart);
                myProcessCallback = new ProcessCallback(ProcessSamples);
                myStopCallback = new StopCallback(OnStop);

                SetThis(sfSoundRecorder_Create(myStartCallback, myProcessCallback, myStopCallback, IntPtr.Zero));
            }

            ////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Tell if the system supports sound capture.
            /// If not, this class won't be usable
            /// </summary>
            ////////////////////////////////////////////////////////////
            public static bool CanCapture
            {
                get { return sfSoundRecorder_CanCapture(); }
            }

            /// <summary>
            /// Sample rate of the recorder, in samples per second
            /// </summary>
            ////////////////////////////////////////////////////////////
            public uint SampleRate
            {
                get { return sfSoundRecorder_GetSampleRate(This); }
            }

            /// <summary>
            /// Handle the destruction of the object
            /// </summary>
            /// <param name="disposing">Is the GC disposing the object, or is it an explicit call ?</param>
            ////////////////////////////////////////////////////////////
            protected override void Destroy(bool disposing)
            {
                sfSoundRecorder_Destroy(This);
            }

            ////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Process a new chunk of recorded samples
            /// </summary>
            /// <param name="samples">Array of samples to process</param>
            /// <returns>False to stop recording audio data, true to continue</returns>
            ////////////////////////////////////////////////////////////
            protected abstract bool OnProcessSamples(short[] samples);

            /// <summary>
            /// Called when a new capture starts
            /// </summary>
            /// <returns>False to abort recording audio data, true to continue</returns>
            ////////////////////////////////////////////////////////////
            protected virtual bool OnStart()
            {
                // Does nothing by default
                return true;
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Called when the current capture stops
            /// </summary>
            ////////////////////////////////////////////////////////////
            protected virtual void OnStop()
            {
                // Does nothing by default
            }

            ////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Function called directly by the C library ; convert
            /// arguments and forward them to the internal virtual function
            /// </summary>
            /// <param name="samples">Pointer to the array of samples</param>
            /// <param name="nbSamples">Number of samples in the array</param>
            /// <param name="userData">User data -- unused</param>
            /// <returns>False to stop recording audio data, true to continue</returns>
            ////////////////////////////////////////////////////////////
            bool ProcessSamples(IntPtr samples, uint nbSamples, IntPtr userData)
            {
                var samplesArray = new short[nbSamples];
                Marshal.Copy(samples, samplesArray, 0, samplesArray.Length);

                return OnProcessSamples(samplesArray);
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Start the capture.
            /// Warning : only one capture can happen at the same time
            /// </summary>
            /// <param name="sampleRate"> Sound frequency; the more samples, the higher the quality (44100 by default = CD quality)</param>
            ////////////////////////////////////////////////////////////
            public void Start(uint sampleRate = 44100u)
            {
                sfSoundRecorder_Start(This, sampleRate);
            }

            ////////////////////////////////////////////////////////////
            /// <summary>
            /// Stop the capture
            /// </summary>
            ////////////////////////////////////////////////////////////
            public void Stop()
            {
                sfSoundRecorder_Stop(This);
            }
        }
    }
}