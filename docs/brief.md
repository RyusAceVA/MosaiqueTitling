# Brief — Mosaïque flottante (Windows 11)

- **Message d'origine** : « logiciel windows 11 de regroupement de fenêtre un groupe qui se comporte comme une fenetre windows classique » ; précisé : « imagine 4 fenêtres d'explorateur en grille de 4 et d'autres fenêtres à côté ou par-dessus ».
- **Utilisateur et situation** : toi, sur ton PC Windows 11 (poste géré, sans droits administrateur), avec plusieurs explorateurs ouverts en même temps.
- **Comment c'est résolu aujourd'hui** : à la main (glisser chaque fenêtre), ou FancyZones / Snap qui découpent l'écran entier mais ne déplacent pas un bloc. Groupy, WindowTabs, TidyTabs font des onglets (une seule fenêtre visible), pas une grille.
- **Version la plus simple qui apporte déjà de la valeur** : un cadre qui contient N fenêtres existantes en grille ; le cadre se déplace, se redimensionne, se réduit et se restaure comme une fenêtre classique ; les autres fenêtres restent libres. Groupe valable **pendant la session seulement** (pas de sauvegarde).
- **Hypothèse qui, si elle est fausse, change le produit** — et l'expérience courte : on ne peut pas insérer proprement les fenêtres d'autres applications dans la sienne ; on les déplace donc en même temps que le cadre. Si c'est saccadé ou si le groupe se « déchire », il faut une autre approche (déplacer au relâchement, ou contour seulement pendant le glisser). Expérience : `experience-mosaique.ps1` (30 min).
- **Contrainte à vérifier en même temps** : le poste géré autorise-t-il PowerShell en mode complet et l'exécution d'un programme non signé ? Sinon le produit ne pourra pas tourner sur ce PC.
- **Résultat de l'expérience (26/11, v3)** : hypothèse **validée pour le déplacement** (fluide, 4 explorateurs, 0 erreur, PowerShell complet + C# compilé autorisés sur le poste géré). **Redimensionnement saccadé** (l'Explorateur redessine à chaque pas) → v1 : réajuster la grille au relâchement ou au plus toutes les ~50 ms. Pièges appris : bordures invisibles de Windows 11 (~7 px, à compenser), ordre d'affichage à rétablir à chaque mouvement, explorateurs à repérer par classe `CabinetWClass` (onglets).
- **Besoin ajouté** : à la fermeture du groupe, choisir « fermer le groupe et ses fenêtres » ou « fermer seulement le groupe (libérer les fenêtres) ».
- **Non-objectifs (v1)** : sauvegarde des groupes, onglets, plusieurs écrans, fenêtres administrateur.
- **Objection retenue** (autre modèle) : à demander à Gemini après l'expérience.

Porte 1 — validé par le propriétaire le : 26/11/2026
