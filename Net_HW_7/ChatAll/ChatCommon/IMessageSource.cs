namespace ChatCommon
{
    public interface IMessageSource<T>
    {
        void Send(MessageUdp message, T toAddr);
        MessageUdp Receive(ref T fromAddr);
        public T CreateNewT();
        public T CopyT(T t);
    }
}