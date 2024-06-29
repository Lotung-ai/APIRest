using Dot.Net.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dot.Net.WebApi.Data
{
    /// <summary>
    /// Implémentation du dépôt pour les opérations CRUD sur l'entité <see cref="BidList"/>.
    /// Cette classe implémente l'interface <see cref="IBidRepository"/> et utilise Entity Framework Core pour interagir avec la base de données.
    /// </summary>
    public class BidRepository : IBidRepository
    {
        private readonly LocalDbContext _context;

        /// <summary>
        /// Constructeur pour injecter le contexte de base de données dans le dépôt.
        /// </summary>
        /// <param name="context">Contexte de la base de données utilisé pour les opérations CRUD.</param>
        public BidRepository(LocalDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crée une nouvelle offre en l'ajoutant à la base de données.
        /// </summary>
        /// <param name="bid">L'objet <see cref="BidList"/> représentant l'offre à ajouter.</param>
        /// <returns>La tâche représentant l'opération asynchrone, avec l'offre ajoutée comme résultat.</returns>
        public async Task<BidList> CreateBidAsync(BidList bid)
        {
            // Ajouter l'offre à la collection Bids du contexte.
            _context.Bids.Add(bid);
            // Sauvegarder les modifications dans la base de données.
            await _context.SaveChangesAsync();
            // Retourner l'offre ajoutée.
            return bid;
        }

        /// <summary>
        /// Récupère une offre spécifique par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant de l'offre à récupérer.</param>
        /// <returns>La tâche représentant l'opération asynchrone, avec l'offre trouvée comme résultat.</returns>
        public async Task<BidList> GetBidByIdAsync(int id)
        {
            // Rechercher l'offre par son identifiant.
            return await _context.Bids.FindAsync(id);
        }

        /// <summary>
        /// Récupère toutes les offres disponibles.
        /// </summary>
        /// <returns>La tâche représentant l'opération asynchrone, avec une collection des offres comme résultat.</returns>
        public async Task<IEnumerable<BidList>> GetAllBidsAsync()
        {
            // Récupérer toutes les offres sous forme de liste.
            return await _context.Bids.ToListAsync();
        }

        /// <summary>
        /// Met à jour une offre existante dans la base de données.
        /// </summary>
        /// <param name="bid">L'objet <see cref="BidList"/> contenant les données mises à jour de l'offre.</param>
        /// <returns>La tâche représentant l'opération asynchrone, avec l'offre mise à jour comme résultat.</returns>
        public async Task<BidList> UpdateBidAsync(BidList bid)
        {
            // Modifier l'état de l'entité pour indiquer qu'elle est en mode de mise à jour.
            _context.Entry(bid).State = EntityState.Modified;
            // Sauvegarder les modifications dans la base de données.
            await _context.SaveChangesAsync();
            // Retourner l'offre mise à jour.
            return bid;
        }

        /// <summary>
        /// Supprime une offre de la base de données par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant de l'offre à supprimer.</param>
        /// <returns>La tâche représentant l'opération asynchrone, avec un booléen indiquant si la suppression a réussi.</returns>
        public async Task<bool> DeleteBidAsync(int id)
        {
            // Rechercher l'offre à supprimer.
            var bid = await _context.Bids.FindAsync(id);
            // Vérifier si l'offre existe.
            if (bid == null) return false;

            // Supprimer l'offre de la collection Bids du contexte.
            _context.Bids.Remove(bid);
            // Sauvegarder les modifications dans la base de données.
            await _context.SaveChangesAsync();
            // Retourner vrai pour indiquer que la suppression a réussi.
            return true;
        }
    }
}
