using System.Threading;
using uEye.Defines;

namespace AresAdditiveDevicesPlugin.UEyeCamera.Impl
{
    public static class MemoryHelper
    {
        public static Status AllocImageMems(uEye.Camera Camera, int nCount)
        {
            var statusRet = Status.SUCCESS;

            for (var i = 0; i < nCount; i++)
            {
                statusRet = Camera.Memory.Allocate();

                if (statusRet != Status.SUCCESS)
                    FreeImageMems(Camera);
            }

            return statusRet;
        }

        public static Status FreeImageMems(uEye.Camera Camera)
        {
            int[] idList;
            var statusRet = Camera.Memory.GetList(out idList);

            if (Status.SUCCESS == statusRet)
                foreach (var nMemID in idList)
                    do
                    {
                        statusRet = Camera.Memory.Free(nMemID);

                        if (Status.SEQ_BUFFER_IS_LOCKED == statusRet)
                        {
                            Thread.Sleep(1);
                            continue;
                        }

                        break;
                    } while (true);

            return statusRet;
        }

        public static Status InitSequence(uEye.Camera Camera)
        {
            int[] idList;
            var statusRet = Camera.Memory.GetList(out idList);

            if (Status.SUCCESS == statusRet)
            {
                statusRet = Camera.Memory.Sequence.Add(idList);

                if (Status.SUCCESS != statusRet)
                    ClearSequence(Camera);
            }

            return statusRet;
        }

        public static Status ClearSequence(uEye.Camera Camera)
        {
            return Camera.Memory.Sequence.Clear();
        }
    }
}
