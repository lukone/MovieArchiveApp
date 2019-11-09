using System;
using System.Linq;
using System.Threading.Tasks;

namespace MovieArchive
{
    public class PersonCardModel
    {
            public PersonDetails PersonDet;

            public PersonCardModel(Person person)
            {
                //Get movie base data from the class movie selected
                PersonDet = new PersonDetails(person);

            }
    
            public async Task<int> GetDetail()
            {
                var DE = new DataExchange();

                PersonDet = await DE.GetPersonDetail(PersonDet);

                return 1;
            }
    }

}
