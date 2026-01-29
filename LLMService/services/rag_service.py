import json
import os
import chromadb
from config import CHROMA_PERSIST_DIR

_collection = None


def initialize_rag():
    global _collection
    client = chromadb.PersistentClient(path=CHROMA_PERSIST_DIR)
    _collection = client.get_or_create_collection(
        name="letsgo_knowledge",
        metadata={"hnsw:space": "cosine"},
    )

    if _collection.count() > 0:
        print(f"[RAG] Collection already has {_collection.count()} documents")
        return

    documents = []
    ids = []
    metadatas = []

    # Index cities
    data_dir = os.path.join(os.path.dirname(os.path.dirname(__file__)), "rag_data")
    cities_path = os.path.join(data_dir, "cities.json")
    with open(cities_path, "r", encoding="utf-8") as f:
        cities_data = json.load(f)

    for i, city in enumerate(cities_data["supported_cities"]):
        doc = f"City: {city['name']} in {city['country']}. {city.get('description', '')}"
        documents.append(doc)
        ids.append(f"city_{i}")
        metadatas.append({"type": "city", "name": city["name"]})

    # Index system knowledge
    knowledge_path = os.path.join(data_dir, "system_knowledge.md")
    with open(knowledge_path, "r", encoding="utf-8") as f:
        knowledge = f.read()

    # Split by sections
    sections = knowledge.split("\n## ")
    for i, section in enumerate(sections):
        section = section.strip()
        if section:
            if not section.startswith("# "):
                section = "## " + section
            documents.append(section)
            ids.append(f"knowledge_{i}")
            metadatas.append({"type": "knowledge"})

    _collection.add(documents=documents, ids=ids, metadatas=metadatas)
    print(f"[RAG] Indexed {len(documents)} documents")


def retrieve_context(query: str, n_results: int = 3) -> str:
    if _collection is None:
        return ""

    results = _collection.query(query_texts=[query], n_results=n_results)

    if not results["documents"] or not results["documents"][0]:
        return ""

    context_parts = results["documents"][0]
    return "\n\n".join(context_parts)
