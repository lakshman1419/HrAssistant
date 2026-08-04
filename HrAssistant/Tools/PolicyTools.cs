using HrAssistant.Services;

namespace HrAssistant.Tools
{
    public class PolicyTools
    {

        private readonly VectorStoreService _vector;


        public PolicyTools(
            VectorStoreService vector)
        {
            _vector = vector;
        }



        public async Task<string> SearchPolicyAsync(
            string question)
        {

            return await _vector.Search(
                question);

        }

    }
}
