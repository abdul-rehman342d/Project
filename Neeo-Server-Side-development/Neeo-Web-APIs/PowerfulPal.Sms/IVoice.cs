namespace PowerfulPal.Sms
{
    public interface IVoice
    {
        IVoice SuccessorApi { get; set; }
        void Call(string phoneNumber, string code);
    }
}
