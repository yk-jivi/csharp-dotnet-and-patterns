CREATE TABLE Celebrities (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Firstname NVARCHAR(100) NOT NULL,
    Surname NVARCHAR(100) NOT NULL,
    PhotoPath NVARCHAR(200) NOT NULL
);

INSERT INTO Celebrities (Firstname, Surname, PhotoPath) VALUES 
('Noam', 'Chomsky', '/Photo/Chomsky.jpg'),
('Donald', 'Knuth', '/Photo/Knuth.jpg'),
('John', 'Backus', '/Photo/Backus.jpg'),
('Niklaus', 'Wirth', '/Photo/Wirth.jpg'),
('James', 'Gosling', '/Photo/Gosling.jpg'),
('Bjarne', 'Stroustrup', '/Photo/Stroustrup.jpg'),
('Edsger', 'Dijkstra', '/Photo/Dijkstra.jpg');

select * from Celebrities
