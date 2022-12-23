namespace Rizing.Interface
{
    public interface IEntity {
        //State changes
        void Play();
        
        void Pause();
        
        //Processors
        void Process(float deltaTime);

        void FixedProcess(float deltaTime);
        
        void LateProcess(float deltaTime);
        
        void LateFixedProcess(float deltaTime);
    }
}