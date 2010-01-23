There is a lot of duplication in this test project, since it specifies it's own mapping and many if not all of the TestDoubles are duplicates.

This is because I would like to keep the SharpArchProviders as segregated as possible because they are useful outside of ClubPool. The only 
ClubPool reference that ClubPool.SharpArchProviders has is to ClubPool.Framework, which is also useful outside of ClubPool and could be
extracted out into a separate project.