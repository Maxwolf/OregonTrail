namespace TrailEntities
{
    /// <summary>
    ///     Ailments which the player entities may come under the influence given enough of a roll chance to infect.
    /// </summary>
    /// <remarks>Typically chances to get diseases go up around other people and sources of water.</remarks>
    public enum Disease
    {
        /// <summary>
        ///     Cholera is an infection of the small intestine by some strains of the bacterium Vibrio cholerae.
        /// </summary>
        Cholera,

        /// <summary>
        ///     Dysentery is an inflammation of the intestine causing diarrhea with blood.
        /// </summary>
        Dysentery,

        /// <summary>
        ///     Fever, also known as pyrexia and febrile response, is defined as having a temperature above the normal range due to
        ///     an increase in the body's temperature.
        /// </summary>
        Fever,

        /// <summary>
        ///     Infection is the invasion of an organism's body tissues by disease-causing agents, their multiplication, and the
        ///     reaction of host tissues to these organisms and the toxins they produce.
        /// </summary>
        Infection,

        /// <summary>
        ///     Any of various diseases or conditions marked by inflammation of the skin, especially lupus vulgaris or lupus
        ///     erythematosus.
        /// </summary>
        Lupus
    }
}