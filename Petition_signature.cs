namespace Petition.Models
{
    public class Petition_signature
    {
        public int PetitionId { get; set; }
        public PetitionTable petition { get; set; }

        public string MemberId { get; set; }
        public Member member { get; set; }

        public string SignatureText { get; set; }
        public DateTime SignedAt { get; set; }
    }
}
